using System.Runtime.CompilerServices;
using Internal.Runtime;

namespace System.Runtime.InteropServices;

public static unsafe class MemoryMarshal 
{
	[Intrinsic]
	public static ref T GetArrayDataReference<T>(T[] array) => ref GetArrayDataReference(array);

	[Intrinsic]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref byte GetArrayDataReference(Array array) => ref Unsafe.AddByteOffset(ref Unsafe.As<RawData>(array).Data, (nuint)array.GetMethodTable()->BaseSize - (nuint)(2 * sizeof(IntPtr)));
}
