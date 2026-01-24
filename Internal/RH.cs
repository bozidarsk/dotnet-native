using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Internal.Runtime;

internal static unsafe partial class RH 
{
	public static void* ReadRelPtr32(void* address) => (byte*)address + *(int*)address;
	public static void WriteRelPtr32(void* dest, void* value) => *(int*)dest = (int)((byte*)value - (byte*)dest);

	private static void ZeroMemory(nint pointer, nuint size) => memset(pointer, 0, size);
	private static void SetMethodTable(nint obj, MethodTable* type) => memcpy(obj, (nint)(&type), (uint)sizeof(nint));



	[RuntimeExport("RhpFallbackFailFast")]
	public static void RhpFallbackFailFast(string message, Exception? e) => abort();

	[RuntimeImport("*", "RhpAssignRef")]
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void RhpAssignRef(ref object address, object obj);

	[RuntimeExport("RhpAssignRef")]
	public static void RhpAssignRef(void** address, void* obj) => *address = obj;

	[RuntimeExport("RhpCheckedAssignRef")]
	public static void RhpCheckedAssignRef(void** address, void* obj) => *address = obj;


	[RuntimeExport("RhpThrowEx")]
	public static void RhpThrowEx(Exception e) => Environment.FailFast("Unhandled exception.", e);

	[RuntimeExport("RhpReversePInvoke")]
	public static void RhpReversePInvoke(nint frame) {}

	[RuntimeExport("RhpReversePInvokeReturn")]
	public static void RhpReversePInvokeReturn(nint frame) {}

	[RuntimeExport("RhpPInvoke")]
	public static void RhpPInvoke(nint frame) {}

	[RuntimeExport("RhpPInvokeReturn")]
	public static void RhpPInvokeReturn(nint frame) {}


	[RuntimeExport("RhpAcquireThunkPoolLock")]
	public static void RhpAcquireThunkPoolLock() {}

	[RuntimeExport("RhpReleaseThunkPoolLock")]
	public static void RhpReleaseThunkPoolLock() {}


	[RuntimeExport("RhBulkMoveWithWriteBarrier")]
	public static void RhBulkMoveWithWriteBarrier(ref byte dmem, ref byte smem, nuint size) => memmove(ref dmem, ref smem, size);

	[RuntimeExport("RhpGcSafeZeroMemory")]
	public static ref byte RhpGcSafeZeroMemory(ref byte dmem, nuint size) 
	{
		ZeroMemory((nint)Unsafe.AsPointer<byte>(ref dmem), size);
		return ref dmem;
	}


	[RuntimeExport("RhpGetDispatchCellInfo")]
	public static void RhpGetDispatchCellInfo(InterfaceDispatchCell* pCell, out DispatchCellInfo cellInfo) => cellInfo = pCell->GetDispatchCellInfo();

	[RuntimeExport("RhpUpdateDispatchCellCache")]
	public static nint RhpUpdateDispatchCellCache(InterfaceDispatchCell* pCell, nint targetCode, MethodTable* pInstanceType, ref DispatchCellInfo cellInfo) 
	{
		return targetCode;
	}

	// [RuntimeExport("RhpSearchDispatchCellCache")]
	// public static nint RhpSearchDispatchCellCache(InterfaceDispatchCell* pCell, MethodTable* pInstanceType) 
	// {

	// }


	[RuntimeExport("RhBox")]
    public static unsafe object RhBox(MethodTable* pEEType, ref byte data)
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

        object result = RhpNewFast(pEEType);

        // Copy the unboxed value type data into the new object.
        // Perform any write barriers necessary for embedded reference fields.
        if (pEEType->ContainsGCPointers)
        {
            RhBulkMoveWithWriteBarrier(ref result.GetRawData(), ref dataAdjustedForNullable, pEEType->ValueTypeSize);
        }
        else
        {
            fixed (byte* pFields = &result.GetRawData())
            fixed (byte* pData = &dataAdjustedForNullable)
                memmove(pFields, pData, pEEType->ValueTypeSize);
        }

        return result;
    }

	[RuntimeExport("RhBoxAny")]
    public static unsafe object RhBoxAny(ref byte data, MethodTable* pEEType)
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
    public static unsafe void RhUnboxAny(object? o, ref byte data, EETypePtr pUnboxToEEType)
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
    public static unsafe ref byte RhUnbox2(MethodTable* pUnboxToEEType, object obj)
    {
        if ((obj == null) || !UnboxAnyTypeCompare(obj.GetMethodTable(), pUnboxToEEType))
        {
            ExceptionIDs exID = obj == null ? ExceptionIDs.NullReference : ExceptionIDs.InvalidCast;
            throw pUnboxToEEType->GetClasslibException(exID);
        }
        return ref obj.GetRawData();
    }

    [RuntimeExport("RhUnboxNullable")]
    public static unsafe void RhUnboxNullable(ref byte data, MethodTable* pUnboxToEEType, object obj)
    {
        if (obj != null && obj.GetMethodTable() != pUnboxToEEType->NullableType)
        {
            throw pUnboxToEEType->GetClasslibException(ExceptionIDs.InvalidCast);
        }
        RhUnbox(obj, ref data, pUnboxToEEType);
    }

	[RuntimeExport("RhUnbox")]
    public static unsafe void RhUnbox(object? obj, ref byte data, MethodTable* pUnboxToEEType)
    {
        // When unboxing to a Nullable the input object may be null.
        if (obj == null)
        {

            // Set HasValue to false and clear the value (in case there were GC references we wish to stop reporting).
            RhpGcSafeZeroMemory(
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
            RhBulkMoveWithWriteBarrier(ref data, ref fields, pEEType->ValueTypeSize);
        }
        else
        {
            // Copy the boxed fields into the new location.
            fixed (byte *pData = &data)
                fixed (byte* pFields = &fields)
                    memmove(pData, pFields, pEEType->ValueTypeSize);
        }
    }

	// [RuntimeExport("RhpGetModuleSection")]
	// public static void* RhpGetModuleSection(TypeManagerHandle* pModule, int headerId, int* length) 
	// {
	// 	return (void*)pModule->AsTypeManager()->GetModuleSection((ReadyToRunSectionType)headerId, length);
	// }

	[RuntimeExport("RhpGetClasslibFunctionFromEEType")]
	public static void* RhpGetClasslibFunctionFromEEType(MethodTable* pEEType, ClassLibFunctionId functionId) 
	{
		return pEEType->TypeManager.AsTypeManager()->GetClasslibFunction(functionId);
	}

	// [RuntimeExport("RhHandleSet")]
	// internal static unsafe void RhHandleSet(nint handle, object? value) 
	// {
	// 	*(nint*)handle = Unsafe.As<object?, nint>(ref value);
	// }

	// [RuntimeExport("RhpGcSafeZeroMemory")]
	// public static nint RhpGcSafeZeroMemory(nint pointer, nuint size) 
	// {
	// 	ZeroMemory(pointer, (uint)size);
	// 	return pointer;
	// }

	// [RuntimeExport("RhpGetDispatchCellInfo")]
	// public static void RhpGetDispatchCellInfo(InterfaceDispatchCell* pCell, out DispatchCellInfo pDispatchCellInfo) 
	// {
	// 	pDispatchCellInfo = pCell->GetDispatchCellInfo();
	// }

	// [RuntimeExport("RhpSearchDispatchCellCache")]
	// public static byte* RhpSearchDispatchCellCache(InterfaceDispatchCell* pCell, MethodTable* pInstanceType) 
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
	// public static nint RhpUpdateDispatchCellCache(InterfaceDispatchCell* pCell, nint pTargetCode, MethodTable* pInstanceType, ref DispatchCellInfo newCellInfo) => pTargetCode;

	[RuntimeExport("RhpNewFast")]
	public static object RhpNewFast(MethodTable* pEEType) 
	{
		var size = pEEType->BaseSize;

		// Round to next power of 8
		if (size % 8 > 0)
			size = ((size / 8) + 1) * 8;

		var data = malloc(size);
		var obj = Unsafe.As<nint, object>(ref data);
		ZeroMemory(data, size);
		SetMethodTable(data, pEEType);

		return obj;
	}

	[RuntimeExport("RhpNewArray")]
	public static object RhpNewArray(MethodTable* pEEType, int length) 
	{
		var size = pEEType->BaseSize + (uint)length * pEEType->ComponentSize;

		// Round to next power of 8
		if (size % 8 > 0)
			size = ((size / 8) + 1) * 8;

		var data = malloc(size);
		var obj = Unsafe.As<nint, object>(ref data);
		ZeroMemory(data, size);
		SetMethodTable(data, pEEType);

		var b = (byte*)data;
		b += sizeof(nint);
		memcpy((nint)b, (nint)(&length), sizeof(int));

		return obj;
	}
}
