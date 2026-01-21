using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Internal.Runtime;

namespace System;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct RuntimeTypeHandle : IEquatable<RuntimeTypeHandle>
{
	private nint value;

	public nint Value => this.value;
	internal bool IsNull => this.value == 0;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(RuntimeTypeHandle handle) => this.value == handle.value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal EETypePtr ToEETypePtr() => new EETypePtr(this.value);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal MethodTable* ToMethodTable() => (MethodTable*)this.value;

	public static RuntimeTypeHandle FromIntPtr(nint value) => new RuntimeTypeHandle(value);
	public static nint ToIntPtr(RuntimeTypeHandle value) => value.Value;

	public static bool operator == (object? left, RuntimeTypeHandle right) => (left is RuntimeTypeHandle) ? right.Equals((RuntimeTypeHandle)left) : false;
	public static bool operator == (RuntimeTypeHandle left, object? right) => (right is RuntimeTypeHandle) ? left.Equals((RuntimeTypeHandle)right) : false;
	public static bool operator != (object? left, RuntimeTypeHandle right) => (left is RuntimeTypeHandle) ? !right.Equals((RuntimeTypeHandle)left) : false;
	public static bool operator != (RuntimeTypeHandle left, object? right) => (right is RuntimeTypeHandle) ? !left.Equals((RuntimeTypeHandle)right) : false;

	public override bool Equals(object? obj) => (obj is RuntimeTypeHandle handle) ? Equals(handle) : false;
	public override int GetHashCode() => IsNull ? 0 : this.ToEETypePtr().GetHashCode();

	internal RuntimeTypeHandle(EETypePtr pEEType) : this(pEEType.RawValue) {}
	internal RuntimeTypeHandle(MethodTable* pEEType) : this((nint)pEEType) {}
	private RuntimeTypeHandle(nint value) => this.value = value;
}
