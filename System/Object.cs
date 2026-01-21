using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime;
using Internal.Runtime;

namespace System;

public unsafe abstract class Object 
{
	private MethodTable* m_pEEType;

	public virtual string ToString() => GetType().ToString();
	public virtual bool Equals(object? other) => this == other;
	public virtual int GetHashCode() => this.ToString().GetHashCode();
	public virtual Type GetType() => Type.GetTypeFromMethodTable(m_pEEType);

	public Object() {}
	~Object() {}

	internal ref byte GetRawData() => ref Unsafe.As<RawData>(this).Data;
	internal uint GetRawDataSize() => GetMethodTable()->BaseSize - (uint)sizeof(ObjHeader) - (uint)sizeof(MethodTable*);
	internal MethodTable* GetMethodTable() => m_pEEType;
	internal ref MethodTable* GetMethodTableRef() => ref m_pEEType;
	internal EETypePtr GetEETypePtr() => new(m_pEEType);

	internal nuint GetRawObjectDataSize()
	{
		var mt = GetMethodTable();

		// See comment on RawArrayData for details
		nuint rawSize = mt->BaseSize - (nuint)(2 * sizeof(nint));

		if (mt->HasComponentSize)
			rawSize += (uint)Unsafe.As<RawArrayData>(this).Length * (nuint)mt->ComponentSize;

		return rawSize;
	}


	public static bool ReferenceEquals(object? a, object? b) => a == b;

	[Intrinsic]
	protected internal object MemberwiseClone() 
	{
		object clone = 
			GetEETypePtr().IsArray
			? InternalCalls.RhpNewArray(GetMethodTable(), Unsafe.As<Array>(this).Length)
			: InternalCalls.RhpNewFast(GetMethodTable())
		;

		// copy contents of "this" to the clone

		uint byteCount = GetMethodTable()->BaseSize - (uint)(2 * sizeof(nint));
		if (GetMethodTable()->HasComponentSize)
			byteCount += (uint)Unsafe.As<RawArrayData>(this).Length * (uint)GetMethodTable()->ComponentSize;

		// nuint byteCount = RuntimeHelpers.GetRawObjectDataSize(this);
		ref byte src = ref this.GetRawData();
		ref byte dst = ref clone.GetRawData();

		Unsafe.CopyBlockUnaligned(ref dst, ref src, byteCount);

		return clone;
	}
}
