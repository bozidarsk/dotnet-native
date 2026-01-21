using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime;

namespace System;

public abstract class Type : MemberInfo
{
	public abstract RuntimeTypeHandle TypeHandle { get; }
	public abstract string FullName { get; }
	public abstract string Name { get; }




	public static Type GetTypeFromHandle(RuntimeTypeHandle runtimeTypeHandle) => new TypeInfo(runtimeTypeHandle);
	internal static unsafe Type GetTypeFromMethodTable(MethodTable* pEEType) => new TypeInfo(new RuntimeTypeHandle(new EETypePtr(pEEType)));

	[Intrinsic] public static bool operator == (Type left, Type right) => RuntimeTypeHandle.ToIntPtr(left.TypeHandle) == RuntimeTypeHandle.ToIntPtr(right.TypeHandle);
	[Intrinsic] public static bool operator != (Type left, Type right) => !(left == right);

	public override bool Equals(object? other) => (other is Type type) ? this == type : false;
	public override int GetHashCode() => this.TypeHandle.GetHashCode();
	public override string ToString() => FullName;
}
