using System.Runtime;
using System.Runtime.InteropServices;

namespace System;

public static class Math 
{
	public const double E = 2.7182818284590452354;
	public const double PI = 3.14159265358979323846;
	public const double Tau = 6.283185307179586476925;

	public static sbyte Abs(sbyte x) => (x < 0) ? (sbyte)-x : x;
	public static short Abs(short x) => (x < 0) ? (short)-x : x;
	public static int Abs(int x) => (x < 0) ? -x : x;
	public static long Abs(long x) => (x < 0) ? -x : x;

	public static double Abs(double x) => RH.fabs(x);
	public static double Floor(double x) => RH.floor(x);
	public static double Ceiling(double x) => RH.ceil(x);
	public static double Sqrt(double x) => RH.sqrt(x);
	public static double Cbrt(double x) => RH.cbrt(x);
	public static double Log2(double x) => RH.log2(x);
	public static double Log10(double x) => RH.log10(x);
	public static double Sin(double x) => RH.sin(x);
	public static double Cos(double x) => RH.cos(x);
	public static double Tan(double x) => RH.tan(x);
	public static double Asin(double x) => RH.asin(x);
	public static double Acos(double x) => RH.acos(x);
	public static double Atan(double x) => RH.atan(x);
	public static double Atan2(double x) => RH.atan2(x);
	public static double Sinh(double x) => RH.sinh(x);
	public static double Cosh(double x) => RH.cosh(x);
	public static double Tanh(double x) => RH.tanh(x);
	public static double Asinh(double x) => RH.asinh(x);
	public static double Acosh(double x) => RH.acosh(x);
	public static double Atanh(double x) => RH.atanh(x);
	public static double FusedMultiplyAdd(double x, double y, double z) => RH.fma(x, y, z);
}
