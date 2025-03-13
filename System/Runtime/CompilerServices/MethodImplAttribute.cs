using System;

namespace System.Runtime.CompilerServices;

public enum MethodImplOptions 
{
    Unmanaged = 0x0004,
    NoInlining = 0x0008,
    NoOptimization = 0x0040,
    AggressiveInlining = 0x0100,
    AggressiveOptimization = 0x200,
    InternalCall = 0x1000,
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
public sealed class MethodImplAttribute : Attribute
{
	internal MethodImplOptions value;

	public MethodImplAttribute() {}
	public MethodImplAttribute(MethodImplOptions methodImplOptions) => value = methodImplOptions;
	public MethodImplAttribute(short value) => this.value = (MethodImplOptions)value;

	public MethodImplOptions Value => value;
}
