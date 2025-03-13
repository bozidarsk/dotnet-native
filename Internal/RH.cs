using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Internal.Runtime;

namespace Internal;

internal static unsafe class RH 
{
	[DllImport("*", EntryPoint = "kmalloc", CallingConvention = CallingConvention.Cdecl)]
	private static extern nint Allocate(uint size);

	[DllImport("*", EntryPoint = "kfree", CallingConvention = CallingConvention.Cdecl)]
	private static extern void Free(nint pointer);

	[DllImport("*", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
	private static extern void SetMemory(nint pointer, byte value, nuint size);

	[DllImport("*", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
	private static extern void CopyMemory(nint dest, nint src, nuint size);

	private static void ZeroMemory(nint pointer, nuint size) => SetMemory(pointer, 0, size);

	private static void CopyMemory(void* dest, void* src, nuint size) => CopyMemory((nint)dest, (nint)src, size);
	private static void CopyMemory(ref byte dest, ref byte src, nuint size) => CopyMemory(Unsafe.AsPointer<byte>(ref dest), Unsafe.AsPointer<byte>(ref src), size);


	[RuntimeExport("RhpAssignRef")]
	private static void RhpAssignRef(void** address, void* obj) => *address = obj;

	[RuntimeExport("RhpByRefAssignRef")]
	private static void RhpByRefAssignRef(void** address, void* obj) => *address = obj;

	[RuntimeExport("RhpCheckedAssignRef")]
	private static void RhpCheckedAssignRef(void** address, void* obj) => *address = obj;


	[RuntimeExport("RhpReversePInvoke")]
	private static void RhpReversePInvoke(nint frame) {}

	[RuntimeExport("RhpReversePInvokeReturn")]
	private static void RhpReversePInvokeReturn(nint frame) {}

	[RuntimeExport("RhpPInvoke")]
	private static void RhpPInvoke(nint frame) {}

	[RuntimeExport("RhpPInvokeReturn")]
	private static void RhpPInvokeReturn(nint frame) {}


	[RuntimeExport("RhpAcquireThunkPoolLock")]
	private static void RhpAcquireThunkPoolLock() {}

	[RuntimeExport("RhpReleaseThunkPoolLock")]
	private static void RhpReleaseThunkPoolLock() {}


	[RuntimeExport("RhBulkMoveWithWriteBarrier")]
	private static void RhBulkMoveWithWriteBarrier(ref byte dmem, ref byte smem, nuint size) => InternalCalls.memmove((byte*)Unsafe.AsPointer<byte>(ref dmem), (byte*)Unsafe.AsPointer<byte>(ref smem), size);

	[RuntimeExport("RhpGcSafeZeroMemory")]
	private static ref byte RhpGcSafeZeroMemory(ref byte dmem, nuint size) 
	{
		ZeroMemory((nint)Unsafe.AsPointer<byte>(ref dmem), size);
		return ref dmem;
	}

	[RuntimeExport("RhpCidResolve")]
	private static unsafe nint RhpCidResolve(nint callerTransitionBlockParam, nint pCell) 
	{
		nint locationOfThisPointer = callerTransitionBlockParam + TransitionBlock.GetThisOffset();
		#pragma warning disable 8500
		object pObject = *(object*)locationOfThisPointer;
		#pragma warning restore 8500


		DispatchCellInfo cellInfo;
		RhpGetDispatchCellInfo((InterfaceDispatchCell*)pCell, out cellInfo);

		MethodTable* pInstanceType = pObject.GetMethodTable();
		nint pTargetCode;

		switch (cellInfo.CellType) 
		{
			case DispatchCellType.InterfaceAndSlot:
				MethodTable* pResolvingInstanceType = pInstanceType;

				pTargetCode = DispatchResolve.FindInterfaceMethodImplementationTarget(
					pResolvingInstanceType,
					cellInfo.InterfaceType.ToPointer(),
					cellInfo.InterfaceSlot,
					ppGenericContext: null
				);

				// if (pTargetCode == 0 && pInstanceType->IsIDynamicInterfaceCastable)
				// {
				// 	var pfnGetInterfaceImplementation = 
				// 		(delegate*<object, MethodTable*, ushort, nint>)pInstanceType->GetClasslibFunction(
				// 			ClassLibFunctionId.IDynamicCastableGetInterfaceImplementation
				// 		);

				// 	pTargetCode = pfnGetInterfaceImplementation(pObject, cellInfo.InterfaceType.ToPointer(), cellInfo.InterfaceSlot);
				// }

				break;
			case DispatchCellType.VTableOffset:
				pTargetCode = *(nint*)(((byte*)pInstanceType) + cellInfo.VTableOffset);
				break;
			default:
				throw new NotSupportedException("!SUPPORTS_NATIVE_METADATA_TYPE_LOADING_AND_SUPPORTS_TOKEN_BASED_DISPATCH_CELLS");
		}

		return (pTargetCode != 0) ? pTargetCode : throw new NullReferenceException();
	}



	[RuntimeExport("RhBox")]
        private static unsafe object RhBox(MethodTable* pEEType, ref byte data)
        {
            ref byte dataAdjustedForNullable = ref data;

            // Can box non-ByRefLike value types only (which also implies no finalizers).

            // If we're boxing a Nullable<T> then either box the underlying T or return null (if the
            // nullable's value is empty).
            if (pEEType->IsNullable)
            {
                // The boolean which indicates whether the value is null comes first in the Nullable struct.
                if (data == 0)
                    return null;

                // Switch type we're going to box to the Nullable<T> target type and advance the data pointer
                // to the value embedded within the nullable.
                dataAdjustedForNullable = ref Unsafe.Add(ref data, pEEType->NullableValueOffset);
                pEEType = pEEType->NullableType;
            }

            object result = InternalCalls.RhpNewFast(pEEType);

            // Copy the unboxed value type data into the new object.
            // Perform any write barriers necessary for embedded reference fields.
            if (pEEType->ContainsGCPointers)
            {
                InternalCalls.RhBulkMoveWithWriteBarrier(ref result.GetRawData(), ref dataAdjustedForNullable, pEEType->ValueTypeSize);
            }
            else
            {
                fixed (byte* pFields = &result.GetRawData())
                fixed (byte* pData = &dataAdjustedForNullable)
                    InternalCalls.memmove(pFields, pData, pEEType->ValueTypeSize);
            }

            return result;
        }

	[RuntimeExport("RhBoxAny")]
        private static unsafe object RhBoxAny(ref byte data, MethodTable* pEEType)
        {
            if (pEEType->IsValueType)
            {
                return RhBox(pEEType, ref data);
            }
            else
            {
                return Unsafe.As<byte, object>(ref data);
            }
        }

	private static unsafe bool UnboxAnyTypeCompare(MethodTable* pEEType, MethodTable* ptrUnboxToEEType)
        {
            if (pEEType == ptrUnboxToEEType)
                return true;

            if (pEEType->ElementType == ptrUnboxToEEType->ElementType)
            {
                // Enum's and primitive types should pass the UnboxAny exception cases
                // if they have an exactly matching cor element type.
                switch (ptrUnboxToEEType->ElementType)
                {
                    case EETypeElementType.Byte:
                    case EETypeElementType.SByte:
                    case EETypeElementType.Int16:
                    case EETypeElementType.UInt16:
                    case EETypeElementType.Int32:
                    case EETypeElementType.UInt32:
                    case EETypeElementType.Int64:
                    case EETypeElementType.UInt64:
                    case EETypeElementType.IntPtr:
                    case EETypeElementType.UIntPtr:
                        return true;
                }
            }

            return false;
        }

	[RuntimeExport("RhUnboxAny")]
        private static unsafe void RhUnboxAny(object? o, ref byte data, EETypePtr pUnboxToEEType)
        {
            MethodTable* ptrUnboxToEEType = (MethodTable*)pUnboxToEEType.ToPointer();
            if (ptrUnboxToEEType->IsValueType)
            {
                bool isValid = false;

                if (ptrUnboxToEEType->IsNullable)
                {
                    isValid = (o == null) || o.GetMethodTable() == ptrUnboxToEEType->NullableType;
                }
                else
                {
                    isValid = (o != null) && UnboxAnyTypeCompare(o.GetMethodTable(), ptrUnboxToEEType);
                }

                if (!isValid)
                {
                    // Throw the invalid cast exception defined by the classlib, using the input unbox MethodTable*
                    // to find the correct classlib.

                    ExceptionIDs exID = o == null ? ExceptionIDs.NullReference : ExceptionIDs.InvalidCast;

                    throw ptrUnboxToEEType->GetClasslibException(exID);
                }

                RhUnbox(o, ref data, ptrUnboxToEEType);
            }
            else
            {
                if (o != null && (TypeCast.IsInstanceOfAny(ptrUnboxToEEType, o) == null))
                {
                    throw ptrUnboxToEEType->GetClasslibException(ExceptionIDs.InvalidCast);
                }

                Unsafe.As<byte, object?>(ref data) = o;
            }
        }

	[RuntimeExport("RhUnbox2")]
        private static unsafe ref byte RhUnbox2(MethodTable* pUnboxToEEType, object obj)
        {
            if ((obj == null) || !UnboxAnyTypeCompare(obj.GetMethodTable(), pUnboxToEEType))
            {
                ExceptionIDs exID = obj == null ? ExceptionIDs.NullReference : ExceptionIDs.InvalidCast;
                throw pUnboxToEEType->GetClasslibException(exID);
            }
            return ref obj.GetRawData();
        }

        [RuntimeExport("RhUnboxNullable")]
        private static unsafe void RhUnboxNullable(ref byte data, MethodTable* pUnboxToEEType, object obj)
        {
            if (obj != null && obj.GetMethodTable() != pUnboxToEEType->NullableType)
            {
                throw pUnboxToEEType->GetClasslibException(ExceptionIDs.InvalidCast);
            }
            RhUnbox(obj, ref data, pUnboxToEEType);
        }

	[RuntimeExport("RhUnbox")]
        private static unsafe void RhUnbox(object? obj, ref byte data, MethodTable* pUnboxToEEType)
        {
            // When unboxing to a Nullable the input object may be null.
            if (obj == null)
            {

                // Set HasValue to false and clear the value (in case there were GC references we wish to stop reporting).
                InternalCalls.RhpGcSafeZeroMemory(
                    ref data,
                    pUnboxToEEType->ValueTypeSize);

                return;
            }

            MethodTable* pEEType = obj.GetMethodTable();

            // Can unbox value types only.

            // A special case is that we can unbox a value type T into a Nullable<T>. It's the only case where
            // pUnboxToEEType is useful.
            if (pUnboxToEEType != null && pUnboxToEEType->IsNullable)
            {

                // Set the first field of the Nullable to true to indicate the value is present.
                Unsafe.As<byte, bool>(ref data) = true;

                // Adjust the data pointer so that it points at the value field in the Nullable.
                data = ref Unsafe.Add(ref data, pUnboxToEEType->NullableValueOffset);
            }

            ref byte fields = ref obj.GetRawData();

            if (pEEType->ContainsGCPointers)
            {
                // Copy the boxed fields into the new location in a GC safe manner
                InternalCalls.RhBulkMoveWithWriteBarrier(ref data, ref fields, pEEType->ValueTypeSize);
            }
            else
            {
                // Copy the boxed fields into the new location.
                fixed (byte *pData = &data)
                    fixed (byte* pFields = &fields)
                        InternalCalls.memmove(pData, pFields, pEEType->ValueTypeSize);
            }
        }

	// [RuntimeExport("RhpGetModuleSection")]
	// private static void* RhpGetModuleSection(TypeManagerHandle* pModule, int headerId, int* length) 
	// {
	// 	return (void*)pModule->AsTypeManager()->GetModuleSection((ReadyToRunSectionType)headerId, length);
	// }

	[RuntimeExport("RhpGetClasslibFunctionFromEEType")]
	private static void* RhpGetClasslibFunctionFromEEType(MethodTable* pEEType, ClassLibFunctionId functionId) 
	{
		return pEEType->TypeManager.AsTypeManager()->GetClasslibFunction(functionId);
	}

	// [RuntimeExport("RhHandleSet")]
	// internal static unsafe void RhHandleSet(nint handle, object? value) 
	// {
	// 	*(nint*)handle = Unsafe.As<object?, nint>(ref value);
	// }

	// [RuntimeExport("RhpGcSafeZeroMemory")]
	// private static nint RhpGcSafeZeroMemory(nint pointer, nuint size) 
	// {
	// 	ZeroMemory(pointer, (uint)size);
	// 	return pointer;
	// }

	[RuntimeExport("RhpGetDispatchCellInfo")]
	private static void RhpGetDispatchCellInfo(InterfaceDispatchCell* pCell, out DispatchCellInfo pDispatchCellInfo) 
	{
		pDispatchCellInfo = pCell->GetDispatchCellInfo();
	}

	// [RuntimeExport("RhpSearchDispatchCellCache")]
	// private static byte* RhpSearchDispatchCellCache(InterfaceDispatchCell* pCell, MethodTable* pInstanceType) 
	// {
	// 	// This function must be implemented in native code so that we do not take a GC while walking the cache
	//     InterfaceDispatchCache* pCache = (InterfaceDispatchCache*)pCell->GetCache();
	//     if (pCache != default)
	//     {
	//         InterfaceDispatchCacheEntry* pCacheEntry = pCache->m_rgEntries;
	//         for (uint i = 0; i < pCache->m_cEntries; i++, pCacheEntry++)
	//             if (pCacheEntry->m_pInstanceType == pInstanceType)
	//                 return (byte*)pCacheEntry->m_pTargetCode;
	//     }

	//     return default;
	// }

	// [RuntimeExport("RhpUpdateDispatchCellCache")]
	// private static nint RhpUpdateDispatchCellCache(InterfaceDispatchCell* pCell, nint pTargetCode, MethodTable* pInstanceType, ref DispatchCellInfo newCellInfo) => pTargetCode;

	[RuntimeExport("RhpNewFast")]
	private static object RhpNewFast(MethodTable* pEEType) 
	{
		var size = pEEType->BaseSize;

		// Round to next power of 8
		if (size % 8 > 0)
			size = ((size / 8) + 1) * 8;

		var data = Allocate(size);
		var obj = Unsafe.As<nint, object>(ref data);
		ZeroMemory(data, size);
		SetMethodTable(data, pEEType);

		return obj;
	}

	[RuntimeExport("RhpNewArray")]
	private static object RhpNewArray(MethodTable* pEEType, int length) 
	{
		var size = pEEType->BaseSize + (uint)length * pEEType->ComponentSize;

		// Round to next power of 8
		if (size % 8 > 0)
			size = ((size / 8) + 1) * 8;

		var data = Allocate(size);
		var obj = Unsafe.As<nint, object>(ref data);
		ZeroMemory(data, size);
		SetMethodTable(data, pEEType);

		var b = (byte*)data;
		b += sizeof(nint);
		CopyMemory((nint)b, (nint)(&length), sizeof(int));

		return obj;
	}

	private static void SetMethodTable(nint obj, MethodTable* type) => CopyMemory(obj, (nint)(&type), (uint)sizeof(nint));
}
