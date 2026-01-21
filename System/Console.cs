using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System;

public static class Console 
{
	public static ConsoleColor ForegroundColor { set; get; } = (ConsoleColor)(-1);
	public static ConsoleColor BackgroundColor { set; get; } = (ConsoleColor)(-1);

	private static unsafe void write(string str) 
	{
		sbyte* chars = stackalloc sbyte[str.Length];

		for (int i = 0; i < str.Length; i++)
			chars[i] = (sbyte)str[i];

		RH.write(1, chars, (nuint)str.Length);
	}

	public static void Clear() => write("\x1b[H\x1b[2J\x1b[3J");

	public static void ResetColor() 
	{
		ForegroundColor = (ConsoleColor)(-1);
		BackgroundColor = (ConsoleColor)(-1);
		write("\x1b[39;49m");
	}

	private static unsafe void WriteString(string? str) 
	{
		if (str == null || str.Length == 0)
			return;

		if (ForegroundColor != (ConsoleColor)(-1) && BackgroundColor != (ConsoleColor)(-1)) 
		{
			// 256 colors by id
			int* colors = stackalloc int[] 
			{
				0, // Black = 0
				4, // DarkBlue = 1
				2, // DarkGreen = 2
				6, // DarkCyan = 3
				1, // DarkRed = 4
				5, // DarkMagenta = 5
				3, // DarkYellow = 6
				7, // Gray = 7
				8, // DarkGray = 8
				12, // Blue = 9
				10, // Green = 10
				14, // Cyan = 11
				9, // Red = 12
				13, // Magenta = 13
				11, // Yellow = 14
				15, // White = 15
			};

			write($"\x1b[38;5;{colors[(int)ForegroundColor & 0xf]}m\x1b[48;5;{colors[(int)BackgroundColor & 0xf]}m");
		}

		write(str);
	}

	public static void Write(string format, params object?[]? args) => WriteString(string.Format(format, args));
	public static void WriteLine(string format, params object?[]? args) 
	{
		WriteString(string.Format(format, args));
		WriteString("\n");
	}

	public static void Write(object? obj) => WriteString(obj?.ToString());
	public static void WriteLine(object? obj) 
	{
		WriteString(obj?.ToString());
		WriteString("\n");
	}

	public static void Write(string? obj) => WriteString(obj);
	public static void WriteLine(string? obj) 
	{
		WriteString(obj);
		WriteString("\n");
	}

	public static void Write(char obj) => WriteString(obj.ToString());
	public static void WriteLine(char obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(byte obj) => WriteString(obj.ToString());
	public static void WriteLine(byte obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(sbyte obj) => WriteString(obj.ToString());
	public static void WriteLine(sbyte obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(ushort obj) => WriteString(obj.ToString());
	public static void WriteLine(ushort obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(short obj) => WriteString(obj.ToString());
	public static void WriteLine(short obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(uint obj) => WriteString(obj.ToString());
	public static void WriteLine(uint obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(int obj) => WriteString(obj.ToString());
	public static void WriteLine(int obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(ulong obj) => WriteString(obj.ToString());
	public static void WriteLine(ulong obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(long obj) => WriteString(obj.ToString());
	public static void WriteLine(long obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(nuint obj) => WriteString(obj.ToString());
	public static void WriteLine(nuint obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}

	public static void Write(nint obj) => WriteString(obj.ToString());
	public static void WriteLine(nint obj) 
	{
		WriteString(obj.ToString());
		WriteString("\n");
	}
}
