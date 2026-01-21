using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Internal.Runtime;

using IDC = Internal.Runtime.InterfaceDispatchCellCachePointerFlags;
using CH = System.Runtime.InterfaceDispatchCacheHeaderFlags;

namespace System.Runtime;

internal enum DispatchCellType 
{
	InterfaceAndSlot = 0x0,
	MetadataToken = 0x1,
	VTableOffset = 0x2,
}

internal enum InterfaceDispatchCacheHeaderFlags : uint
{
	TypeAndSlotIndex = 0x0,
	MetadataToken = 0x1,
	Mask = 0x3,
	Shift = 0x2,
}

internal struct DispatchCellInfo 
{
	public DispatchCellType CellType;
	public EETypePtr InterfaceType;
	public ushort InterfaceSlot;
	public byte HasCache;
	public uint MetadataToken;
	public uint VTableOffset;
}

internal unsafe struct InterfaceDispatchCache 
{
	public InterfaceDispatchCacheHeader m_cacheHeader;
	public InterfaceDispatchCell* m_pCell;
	public uint m_cEntries;
	public InterfaceDispatchCacheEntry* m_rgEntries;
}

internal unsafe struct InterfaceDispatchCacheEntry 
{
	public MethodTable* m_pInstanceType;
	public void* m_pTargetCode;
}

internal unsafe struct InterfaceDispatchCell 
{
	public nuint m_pStub;
	public nuint m_pCache;

	public DispatchCellInfo GetDispatchCellInfo() 
	{
		// Capture m_pCache into a local for safe access (this is a volatile read of a value that may be
		// modified on another thread while this function is executing.)
		IDC cachePointerValue = (IDC)m_pCache;
		DispatchCellInfo cellInfo = new();

		if ((cachePointerValue < IDC.MaxVTableOffsetPlusOne) && ((cachePointerValue & IDC.CachePointerMask) == IDC.CachePointerPointsAtCache)) 
		{
			cellInfo.VTableOffset = (uint)cachePointerValue;
			cellInfo.CellType = DispatchCellType.VTableOffset;
			cellInfo.HasCache = 1;
			return cellInfo;
		}

		// If there is a real cache pointer, grab the data from there.
		if ((cachePointerValue & IDC.CachePointerMask) == IDC.CachePointerPointsAtCache) 
		{
			return ((InterfaceDispatchCacheHeader*)(nuint)cachePointerValue)->GetDispatchCellInfo();
		}

		// Otherwise, walk to cell with Flags and Slot field

		// The slot number/flags for a dispatch cell is encoded once per run of DispatchCells
		// The run is terminated by having an dispatch cell with a null stub pointer.
		InterfaceDispatchCell* currentCell = (InterfaceDispatchCell*)Unsafe.AsPointer<InterfaceDispatchCell>(ref this);
		while (currentCell->m_pStub != 0) 
		{
			currentCell = currentCell + 1;
		}
		nuint cachePointerValueFlags = currentCell->m_pCache;

		DispatchCellType cellType = (DispatchCellType)(cachePointerValueFlags >> 16);
		cellInfo.CellType = cellType;

		if (cellType == DispatchCellType.InterfaceAndSlot) 
		{
			cellInfo.InterfaceSlot = (ushort)cachePointerValueFlags;

			switch (cachePointerValue & IDC.CachePointerMask) 
			{
				case IDC.CachePointerIsInterfacePointerOrMetadataToken:
					cellInfo.InterfaceType = new EETypePtr((MethodTable*)(nuint)(cachePointerValue & ~IDC.CachePointerMask));
					break;

				case IDC.CachePointerIsInterfaceRelativePointer:
				case IDC.CachePointerIsIndirectedInterfaceRelativePointer:
					{
						nuint interfacePointerValue = (nuint)((byte*)Unsafe.AsPointer<nuint>(ref m_pCache) + (int)cachePointerValue);
						interfacePointerValue &= unchecked((nuint)(~IDC.CachePointerMask));
						if ((cachePointerValue & IDC.CachePointerMask) == IDC.CachePointerIsInterfaceRelativePointer) 
						{
							cellInfo.InterfaceType = new EETypePtr((MethodTable*)interfacePointerValue);
						}
						else
						{
							cellInfo.InterfaceType = new EETypePtr(*(MethodTable**)interfacePointerValue);
						}
					}
					break;
			}
		}
		else 
		{
			cellInfo.MetadataToken = (uint)((uint)cachePointerValue >> (int)IDC.CachePointerMaskShift);
		}

		return cellInfo;
	}

	public static bool IsCache(nuint value)
	{
		if ((((IDC)value & IDC.CachePointerMask) != 0) || ((IDC)value < IDC.MaxVTableOffsetPlusOne))
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	public InterfaceDispatchCacheHeader* GetCache()
	{
		// Capture m_pCache into a local for safe access (this is a volatile read of a value that may be
		// modified on another thread while this function is executing.)
		nuint cachePointerValue = m_pCache;
		if (IsCache(cachePointerValue))
		{
			return (InterfaceDispatchCacheHeader*)cachePointerValue;
		}
		else
		{
			return (InterfaceDispatchCacheHeader*)0;
		}
	}
}

internal unsafe struct InterfaceDispatchCacheHeader 
{
	public MethodTable* m_pInterfaceType;
	public InterfaceDispatchCacheHeaderFlags m_slotIndexOrMetadataTokenEncoded;

	public DispatchCellInfo GetDispatchCellInfo() 
	{
		DispatchCellInfo cellInfo = new();

		if ((m_slotIndexOrMetadataTokenEncoded & CH.Mask) == CH.TypeAndSlotIndex) 
		{
			cellInfo.InterfaceType = new EETypePtr(m_pInterfaceType);
			cellInfo.InterfaceSlot = (ushort)((uint)m_slotIndexOrMetadataTokenEncoded >> (int)CH.Shift);
			cellInfo.CellType = DispatchCellType.InterfaceAndSlot;
		}
		else 
		{
			cellInfo.MetadataToken = (uint)m_slotIndexOrMetadataTokenEncoded >> (int)CH.Shift;
			cellInfo.CellType = DispatchCellType.MetadataToken;
		}

		cellInfo.HasCache = 1;
		return cellInfo;
	}
}

internal enum ClassLibFunctionId 
{
	GetRuntimeException = 0,
	FailFast = 1,
	AppendExceptionStackFrame = 3,
	GetSystemArrayEEType = 5,
	OnFirstChance = 6,
	OnUnhandledException = 7,
	IDynamicCastableIsInterfaceImplemented = 8,
	IDynamicCastableGetInterfaceImplementation = 9,
	ObjectiveCMarshalTryGetTaggedMemory = 10,
	ObjectiveCMarshalGetIsTrackedReferenceCallback = 11,
	ObjectiveCMarshalGetOnEnteredFinalizerQueueCallback = 12,
	ObjectiveCMarshalGetUnhandledExceptionPropagationHandler = 13,
}
