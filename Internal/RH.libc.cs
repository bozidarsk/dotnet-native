using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

internal static unsafe partial class RH 
{
	[DllImport("*")]
	public static extern nint write(int fd, void* buffer, nuint size);

	[DllImport("*")]
	public static extern nint malloc(uint size);

	[DllImport("*")]
	public static extern void free(nint pointer);

	[DllImport("*")]
	public static extern void memset(nint pointer, byte value, nuint size);

	[DllImport("*")]
	public static extern void memcpy(nint dest, nint src, nuint size);
	public static void memcpy(void* dest, void* src, nuint size) => memcpy((nint)dest, (nint)src, size);
	public static void memcpy(ref byte dest, ref byte src, nuint size) => memcpy(Unsafe.AsPointer<byte>(ref dest), Unsafe.AsPointer<byte>(ref src), size);

	[DllImport("*")]
	public static extern void memmove(nint dest, nint src, nuint size);
	public static void memmove(void* dest, void* src, nuint size) => memmove((nint)dest, (nint)src, size);
	public static void memmove(ref byte dest, ref byte src, nuint size) => memmove(Unsafe.AsPointer<byte>(ref dest), Unsafe.AsPointer<byte>(ref src), size);

	[DllImport("*")]
	public static extern void abort();
}
