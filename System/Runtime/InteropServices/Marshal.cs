using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public static class Marshal 
{
	public static nint GetFunctionPointerForDelegate<T>(T method) where T : Delegate => method.m_functionPointer;
	public static nint GetFunctionPointerForDelegate(Delegate method) => method.m_functionPointer;

	public static int SizeOf<T>() => Unsafe.SizeOf<T>();

	public static unsafe void PtrToStructure<T>(nint pointer, T structure) => Unsafe.CopyBlockUnaligned(ref Unsafe.As<T, byte>(ref structure), ref Unsafe.AsRef<byte>((void*)pointer), (uint)Unsafe.SizeOf<T>());
	public static unsafe void StructureToPtr<T>(T structure, nint pointer, bool deleteOld) 
	{
		Unsafe.CopyBlockUnaligned((void*)pointer, Unsafe.AsPointer(ref structure), (uint)Unsafe.SizeOf<T>());

		if (deleteOld)
			DestroyStructure<T>((nint)Unsafe.AsPointer(ref structure));
	}

	public static void DestroyStructure<T>(nint pointer) => throw new NotImplementedException();

	internal static bool IsPinnable(object? o) => (o == null) || !o.GetEETypePtr().ContainsGCPointers;
}
