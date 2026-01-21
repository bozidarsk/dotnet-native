#pragma warning disable CS8500

using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Internal.Runtime;

namespace Internal.Runtime.CompilerHelpers;

internal static unsafe class StartupCodeHelpers 
{
	private static TypeManager[] modules;
	private static object[] gcStaticBaseSpines;

	[RuntimeExport("InitializeModules")]
	private static void InitializeModules(nint osModule, ReadyToRunHeader** pModuleHeaders, int count, nint* pClasslibFunctions, int nClasslibFunctions) 
	{
		// dont use any static fields untill after InitailizeGlobalTables (GCStatics)

		var modules = new TypeManager[count];
		var gcStaticBaseSpines = new object[count];

		for (int i = 0; i < count; i++)
			modules[i] = new TypeManager(osModule, pModuleHeaders[i], pClasslibFunctions, (uint)nClasslibFunctions);

		for (int i = 0; i < count; i++)
			InitailizeGlobalTables(modules, i, gcStaticBaseSpines);

		for (int i = 0; i < count; i++) 
		{
			nint data = modules[i].GetModuleSection(ReadyToRunSectionType.DehydratedData, out int length);
			if (data != 0) RehydrateData(data, length);
		}

		for (int i = 0; i < count; i++)
			RunInitializers(modules, i, ReadyToRunSectionType.EagerCctor);

		for (int i = 0; i < count; i++)
			RunInitializers(modules, i, ReadyToRunSectionType.ModuleInitializerList);

		StartupCodeHelpers.modules = modules;
		StartupCodeHelpers.gcStaticBaseSpines = gcStaticBaseSpines;
	}

	private static void RunInitializers(TypeManager[] modules, int moduleIndex, ReadyToRunSectionType section) 
	{
		byte* pInitializers = (byte*)modules[moduleIndex].GetModuleSection(section, out int length);

		for (
			byte* pCurrent = pInitializers;
			pCurrent < (pInitializers + length);
			pCurrent += MethodTable.SupportsRelativePointers ? sizeof(int) : sizeof(nint))
		{
			var initializer = MethodTable.SupportsRelativePointers ? (delegate*<void>)RH.ReadRelPtr32(pCurrent) : *(delegate*<void>*)pCurrent;
			initializer();
		}
	}

	private static void InitailizeGlobalTables(TypeManager[] modules, int moduleIndex, object[] gcStaticBaseSpines) 
	{
		// Configure the module indirection cell with the newly created TypeManager. This allows EETypes to find
		// their interface dispatch map tables.

		int length;
		TypeManagerSlot* section = (TypeManagerSlot*)modules[moduleIndex].GetModuleSection(ReadyToRunSectionType.TypeManagerIndirection, out length);

		section->TypeManager = new((nint)Unsafe.AsPointer(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(modules), moduleIndex)));
		section->ModuleIndex = moduleIndex;

		// Initialize statics if any are present
		nint gcStatics = modules[moduleIndex].GetModuleSection(ReadyToRunSectionType.GCStaticRegion, out length);
		if (gcStatics != 0) 
		{
			// Debug.Assert(length % (MethodTable.SupportsRelativePointers ? sizeof(int) : sizeof(nint)) == 0);

			object[] spine = InitializeStatics(gcStatics, length);

			// Call write barrier directly. Assigning object reference does a type check.
			// Debug.Assert((uint)moduleIndex < (uint)gcStaticBaseSpines.Length);

			ref object rawSpineIndexData = ref Unsafe.As<byte, object>(ref Unsafe.As<RawArrayData>(gcStaticBaseSpines).Data);
			Unsafe.Add(ref rawSpineIndexData, moduleIndex) = spine;
		}

		// Initialize frozen object segment for the module with GC present
		nint frozenObjectSection = modules[moduleIndex].GetModuleSection(ReadyToRunSectionType.FrozenObjectRegion, out length);
		if (frozenObjectSection != 0) 
		{
			// Debug.Assert(length % sizeof(nint));
			InitializeFrozenObjectSegment(frozenObjectSection, length);
		}
	}

	private static unsafe void InitializeFrozenObjectSegment(nint segmentStart, int length) 
    {
    	// maybe important ??


		// if (RuntimeImports.RhRegisterFrozenSegment((void*)segmentStart, (nuint)length, (nuint)length, (nuint)length) == 0)
		// 	Environment.FailFast("Failed to register frozen object segment for the module.");
	}

	private static object[] InitializeStatics(nint gcStaticRegionStart, int length) 
	{
		byte* gcStaticRegionEnd = (byte*)gcStaticRegionStart + length;

		object[] spine = new object[length / (MethodTable.SupportsRelativePointers ? sizeof(int) : sizeof(nint))];
		ref object rawSpineData = ref MemoryMarshal.GetArrayDataReference(spine);

		int currentBase = 0;

		for (
			byte* block = (byte*)gcStaticRegionStart;
			block < gcStaticRegionEnd;
			block += MethodTable.SupportsRelativePointers ? sizeof(int) : sizeof(nint)
		)
		{
			// Gc Static regions can be shared by modules linked together during compilation. To ensure each
			// is initialized once, the static region pointer is stored with lowest bit set in the image.
			// The first time we initialize the static region its pointer is replaced with an object reference
			// whose lowest bit is no longer set.

			nint* pBlock = MethodTable.SupportsRelativePointers ? (nint*)RH.ReadRelPtr32(block) : *(nint**)block;
			nint blockAddress = MethodTable.SupportsRelativePointers ? (nint)RH.ReadRelPtr32(pBlock) : *pBlock;

			if ((blockAddress & GCStaticRegionConstants.Uninitialized) == GCStaticRegionConstants.Uninitialized) 
            {
				// object? obj = null;
				// RH.RhAllocateNewObject(blockAddress & ~GCStaticRegionConstants.Mask, (uint)GC_ALLOC_FLAGS.GC_ALLOC_PINNED_OBJECT_HEAP, &obj);

				object obj = RH.RhpNewFast((MethodTable*)(blockAddress & ~GCStaticRegionConstants.Mask));

				if (obj == null)
					Environment.FailFast("Failed allocating GC static bases.");

				if ((blockAddress & GCStaticRegionConstants.HasPreInitializedData) == GCStaticRegionConstants.HasPreInitializedData) 
				{
					// The next pointer is preinitialized data blob that contains preinitialized static GC fields,
					// which are pointer relocs to GC objects in frozen segment.
					// It actually has all GC fields including non-preinitialized fields and we simply copy over the
					// entire blob to this object, overwriting everything.
					void* pPreInitDataAddr = MethodTable.SupportsRelativePointers ? RH.ReadRelPtr32((int*)pBlock + 1) : (void*)*(pBlock + 1);
					RH.RhBulkMoveWithWriteBarrier(ref obj!.GetRawData(), ref *(byte*)pPreInitDataAddr, obj!.GetRawObjectDataSize());
				}

				// Call write barrier directly. Assigning object reference does a type check.
				Unsafe.Add(ref rawSpineData, currentBase) = obj!;

				// Update the base pointer to point to the pinned object
				*pBlock = *(nint*)&obj;
			}

			currentBase++;
		}

		return spine;
	}

	private static void RehydrateData(nint dehydratedData, int length) 
	{
		// Destination for the hydrated data is in the first 32-bit relative pointer
		byte* pDest = (byte*)RH.ReadRelPtr32((void*)dehydratedData);

		// The dehydrated data follows
		byte* pCurrent = (byte*)dehydratedData + sizeof(int);
		byte* pEnd = (byte*)dehydratedData + length;

		// Fixup table immediately follows the command stream
		int* pFixups = (int*)pEnd;

		while (pCurrent < pEnd) 
		{
			pCurrent = DehydratedDataCommand.Decode(pCurrent, out int command, out int payload);
			switch (command)
			{
				case DehydratedDataCommand.Copy:
					if (payload < 4)
					{
						*pDest = *pCurrent;
						if (payload > 1)
							*(short*)(pDest + payload - 2) = *(short*)(pCurrent + payload - 2);
					}
					else if (payload < 8)
					{
						*(int*)pDest = *(int*)pCurrent;
						*(int*)(pDest + payload - 4) = *(int*)(pCurrent + payload - 4);
					}
					else if (payload <= 16)
					{
#if !BITS32
						*(long*)pDest = *(long*)pCurrent;
						*(long*)(pDest + payload - 8) = *(long*)(pCurrent + payload - 8);
#else
						*(int*)pDest = *(int*)pCurrent;
						*(int*)(pDest + 4) = *(int*)(pCurrent + 4);
						*(int*)(pDest + payload - 8) = *(int*)(pCurrent + payload - 8);
						*(int*)(pDest + payload - 4) = *(int*)(pCurrent + payload - 4);
#endif
					}
					else
					{
						// At the time of writing this, 90% of DehydratedDataCommand.Copy cases
						// would fall into the above specialized cases. 10% fall back to memmove.
						RH.memmove(pDest, pCurrent, (nuint)payload);
					}

					pDest += payload;
					pCurrent += payload;
					break;
				case DehydratedDataCommand.ZeroFill:
					pDest += payload;
					break;
				case DehydratedDataCommand.PtrReloc:
					*(void**)pDest = RH.ReadRelPtr32(pFixups + payload);
					pDest += sizeof(void*);
					break;
				case DehydratedDataCommand.RelPtr32Reloc:
					RH.WriteRelPtr32(pDest, RH.ReadRelPtr32(pFixups + payload));
					pDest += sizeof(int);
					break;
				case DehydratedDataCommand.InlinePtrReloc:
					while (payload-- > 0)
					{
						*(void**)pDest = RH.ReadRelPtr32(pCurrent);
						pDest += sizeof(void*);
						pCurrent += sizeof(int);
					}
					break;
				case DehydratedDataCommand.InlineRelPtr32Reloc:
					while (payload-- > 0)
					{
						RH.WriteRelPtr32(pDest, RH.ReadRelPtr32(pCurrent));
						pDest += sizeof(int);
						pCurrent += sizeof(int);
					}
					break;
			}
		}
	}
}
