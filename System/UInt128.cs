using System.Runtime.InteropServices;

namespace System;

[StructLayout(LayoutKind.Sequential)]
public readonly struct UInt128 
{
	#if BIGENDIAN
	private readonly ulong upper;
	private readonly ulong lower;
	#else
	private readonly ulong lower;
	private readonly ulong upper;
	#endif

	public static Int128 Zero => new Int128(0, 0);
	public static Int128 MinValue => new Int128(0, 0);
	public static Int128 MaxValue => new Int128(ulong.MaxValue, ulong.MaxValue);

	private const int MaxStringLength = 39;

	public static Int128 Parse(string? str) => TryParse(str, out Int128 result) ? result : throw new FormatException();
	public static unsafe bool TryParse(string? str, out Int128 result) 
	{
		result = new(0, 0);

		if (str == null || str.Length == 0 || (str.Length > MaxStringLength && str[0] != '-') || (str.Length > MaxStringLength + 1 && str[0] == '-'))
			return false;

		ulong high = 0, low = 0;

		for (int i = 0; i < str.Length; i++) 
		{
			if (!char.IsAsciiDigit(str[i]))
				return false;

			if (i < MaxStringLength) 
			{
				low *= 10;
				low += (ulong)(str[i] - 0x30);
			}
			else 
			{
				high *= 10;
				high += (ulong)(str[i] - 0x30);
			}

		}

		result = new(high, low);

		return true;
	}

	public override int GetHashCode() => (int)upper ^ (int)lower;
	public override unsafe string ToString() 
	{
		char* buffer = stackalloc char[MaxStringLength + 1];
		buffer += MaxStringLength + 1;
		*buffer = '\0';

		ulong high = upper;
		ulong low = lower;

		do
		{
			buffer--;
			*buffer = (char)(Math.Abs((int)(lower % 10)) + 0x30);
			low /= 10;
		} while (low != 0);

		do
		{
			buffer--;
			*buffer = (char)(Math.Abs((int)(upper % 10)) + 0x30);
			high /= 10;
		} while (high != 0);

		return new string(buffer);
	}

	public UInt128(ulong lower, ulong upper) => (this.lower, this.upper) = (lower, upper);
}
