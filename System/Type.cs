using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime;

namespace System;

public class Type 
{
	private readonly RuntimeTypeHandle typeHandle;

	public RuntimeTypeHandle TypeHandle => typeHandle;

	public static Type GetTypeFromHandle(RuntimeTypeHandle rh) => new Type(rh);
	internal static unsafe Type GetTypeFromMethodTable(MethodTable* pEEType) => new Type(new RuntimeTypeHandle(new EETypePtr(pEEType)));

	[Intrinsic] public static bool operator == (Type left, Type right) => RuntimeTypeHandle.ToIntPtr(left.typeHandle) == RuntimeTypeHandle.ToIntPtr(right.typeHandle);
	[Intrinsic] public static bool operator != (Type left, Type right) => !(left == right);

	public override bool Equals(object? o) => o is Type && this == (Type)o;

	public override int GetHashCode() => typeHandle.GetHashCode();

	public override string ToString() => "System.Type";

	private Type(RuntimeTypeHandle typeHandle) => this.typeHandle = typeHandle;
}
