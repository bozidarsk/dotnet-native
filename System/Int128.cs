using System.Runtime.InteropServices;

namespace System;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Int128 
{
	#if BIGENDIAN
	private readonly ulong upper;
	private readonly ulong lower;
	#else
	private readonly ulong lower;
	private readonly ulong upper;
	#endif

	public static Int128 Zero => new Int128(0, 0);
	public static Int128 MinValue => new Int128(0x8000_0000_0000_0000, 0);
	public static Int128 MaxValue => new Int128(0x7fff_ffff_ffff_ffff, 0xffff_ffff_ffff_ffff);

	private const int MaxStringLength = 39;

	public static Int128 Parse(string? str) => TryParse(str, out Int128 result) ? result : throw new FormatException();
	public static unsafe bool TryParse(string? str, out Int128 result) 
	{
		result = new(0, 0);

		if (str == null || str.Length == 0 || (str.Length > MaxStringLength && str[0] != '-') || (str.Length > MaxStringLength + 1 && str[0] == '-'))
			return false;

		bool isNegative = str[0] == '-';
		ulong high = 0, low = 0;

		for (int i = isNegative ? 1 : 0; i < str.Length; i++) 
		{
			if (!char.IsAsciiDigit(str[i]))
				return false;

			if (i + (isNegative ? 1 : 0) < MaxStringLength) 
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

		if (isNegative) 
		{
			high = ~high;
			low = ~low;
			low++;

			if (low == 0)
				high++;
		}

		result = new(high, low);

		return true;
	}

	public override int GetHashCode() => (int)upper ^ (int)lower;
	public override unsafe string ToString() 
	{
		char* buffer = stackalloc char[1 + MaxStringLength + 1];
		buffer += 1 + MaxStringLength + 1;
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

		if ((upper >> 63) != 0 || (upper == 0 && (lower >> 63) != 0)) 
		{
			buffer--;
			*buffer = '-';
		}

		return new string(buffer);
	}

	public Int128(ulong upper, ulong lower) => (this.lower, this.upper) = (lower, upper);
}
