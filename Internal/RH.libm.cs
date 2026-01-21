using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

internal static unsafe partial class RH 
{
	[DllImport("*")]
	public static extern double fabs(double x);

	[DllImport("*")]
	public static extern double floor(double x);

	[DllImport("*")]
	public static extern double ceil(double x);

	[DllImport("*")]
	public static extern double sqrt(double x);

	[DllImport("*")]
	public static extern double cbrt(double x);

	[DllImport("*")]
	public static extern double log2(double x);

	[DllImport("*")]
	public static extern double log10(double x);

	[DllImport("*")]
	public static extern double sin(double x);

	[DllImport("*")]
	public static extern double cos(double x);

	[DllImport("*")]
	public static extern double tan(double x);

	[DllImport("*")]
	public static extern double asin(double x);

	[DllImport("*")]
	public static extern double acos(double x);

	[DllImport("*")]
	public static extern double atan(double x);

	[DllImport("*")]
	public static extern double atan2(double x);

	[DllImport("*")]
	public static extern double sinh(double x);

	[DllImport("*")]
	public static extern double cosh(double x);

	[DllImport("*")]
	public static extern double tanh(double x);

	[DllImport("*")]
	public static extern double asinh(double x);

	[DllImport("*")]
	public static extern double acosh(double x);

	[DllImport("*")]
	public static extern double atanh(double x);

	[DllImport("*")]
	public static extern double fma(double x, double y, double z);
}
