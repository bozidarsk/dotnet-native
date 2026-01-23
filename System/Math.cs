namespace System;

public static class Math 
{
	public const double E = 2.7182818284590452354;
	public const double PI = 3.14159265358979323846;
	public const double Tau = 6.283185307179586476925;

	private const double log2 = 0.6931471805599453;
	private const double log10 = 2.302585092994046;

	public static sbyte Abs(sbyte x) => (x < 0) ? (sbyte)-x : x;
	public static short Abs(short x) => (x < 0) ? (short)-x : x;
	public static int Abs(int x) => (x < 0) ? -x : x;
	public static long Abs(long x) => (x < 0) ? -x : x;
	public static unsafe double Abs(double x) 
	{
		*(ulong*)(&x) &= ~(1ul << 63);
		return x;
	}

	public static double Floor(double x) 
	{
		if (double.IsNaN(x) || double.IsInfinity(x) || x == 0)
			return x;

		double t = (double)(long)x;

		if (t > x)
			t -= 1.0;

		return t;
	}

	public static double Ceiling(double x) 
	{
		if (double.IsNaN(x) || double.IsInfinity(x) || x == 0)
			return x;

		double t = (double)(long)x;

		if (t < x)
			t += 1;

		return t;
	}

	public static double FusedMultiplyAdd(double x, double y, double z) => (x * y) + z;
	public static double Sqrt(double x) => Pow(x, 1d / 2d);
	public static double Cbrt(double x) => Pow(x, 1d / 3d);

	public static unsafe double Pow(double x, double y) 
	{
		if (double.IsNaN(x) || double.IsNaN(y))
			return double.NaN;

		if (x == 0 || x == 1)
			return x;

		bool reciprocal = y < 0;
		y = Abs(y);

		int sign = 0;

		if (x < 0) 
		{
			if ((double)(long)y != y)
				return double.NaN;

			sign = (int)((long)y & 1);
			x = Abs(x);
		}

		double result = Exp(Log(x) * y);

		if (reciprocal)
			result = 1 / result;

		result = Abs(result);
		*(ulong*)(&result) |= (ulong)sign << 63;

		return result;
	}

	public static double Exp(double x) 
	{
		if (double.IsNaN(x))
			return double.NaN;

		long integer = (long)x;
		double fraction = x - (double)integer;
		x = fraction;

		double a = x;
		double b = 1;
		double sum = 1;

		for (int i = 1; i <= 10; i++) 
		{
			b *= (double)i;
			sum += a / b;
			a *= x;
		}

		// x = (integer)a + (fraction)b // modf
		// e^(a+b)
		// e^a * e^b

		if (integer > 0) 
		{
			for (long i = 0; i < integer; i++)
				sum *= E;
		}
		else if (integer < 0) 
		{
			integer = -integer;
			for (long i = 0; i < integer; i++)
				sum /= E;
		}

		return sum;
	}

	public static double Log(double x) 
	{
		if (double.IsNaN(x))
			return double.NaN;

		long exponent = 0;
		while (x < -1 || x > 1) 
		{
			x /= 2;
			exponent++;
		}

		x = 1 - x;

		double a = x;
		double sum = 0;

		for (int i = 1; i <= 200; i++) 
		{
			sum -= a / (double)i;
			a *= x;
		}

		// ln(x*2^exponent)
		// ln(x) + ln(2^exponent)
		// ln(x) + exponent*ln(2)

		return sum + (double)exponent * log2;
	}

	public static double Log2(double x) => Log(x) / log2;
	public static double Log10(double x) => Log(x) / log10;
	public static double Log(double x, int logBase) => Log(x) / Log(logBase);

	public static double Sin(double x) 
	{
		if (double.IsNaN(x))
			return double.NaN;

		x /= Tau;
		x = (x - (double)(long)x) * Tau;

		double a = x;
		double b = 1;
		double sum = x;

		for (int i = 1; i <= 15; i++) 
		{
			a *= x * x;
			b *= (double)(2*i);
			b *= (double)(2*i + 1);

			if ((i & 1) == 0)
				sum += a / b;
			else
				sum -= a / b;
		}

		return sum;
	}

	public static double Cos(double x) 
	{
		if (double.IsNaN(x))
			return double.NaN;

		x /= Tau;
		x = (x - (double)(long)x) * Tau;

		double a = 1;
		double b = 1;
		double sum = 1;

		for (int i = 1; i <= 15; i++) 
		{
			a *= x * x;
			b *= (double)(2*i);
			b *= (double)(2*i - 1);

			if ((i & 1) == 0)
				sum += a / b;
			else
				sum -= a / b;
		}

		return sum;
	}

	public static double Tan(double x) => Sin(x) / Cos(x);

	public static unsafe double Asin(double x) 
	{
		if (double.IsNaN(x) || x < -1 || x > 1)
			return double.NaN;

		if (x == 1)
			return PI / 2;

		if (x == -1)
			return -PI / 2;

		delegate*<double, double>* derivatives = stackalloc delegate*<double, double>[] 
		{
			&derivative1,
			&derivative2,
			&derivative3,
			&derivative4,
			&derivative5,
			&derivative6,
			&derivative7,
			&derivative8,
			&derivative9,
		};

		double* arcsinA = stackalloc double[] 
		{
			0.000000000000000, // asin(0.0)
			0.100167421161560, // asin(0.1)
			0.201357920790331, // asin(0.2)
			0.304692654015398, // asin(0.3)
			0.411516846067488, // asin(0.4)
			0.523598775598299, // asin(0.5)
			0.643501108793284, // asin(0.6)
			0.775397496610753, // asin(0.7)
			0.927295218001612, // asin(0.8)
			1.119769514998630, // asin(0.9)
			1.570796326794900, // asin(1.0)
		};

		bool isNegative = x < 0;
		x = Abs(x);

		int index = (int)(x * 10) % 10;
		if ((int)(x * 100) % 10 >= 5) index++;
		if (index == 10) index = 9;

		double value = (double)index / 10;

		double a = (x - value);
		double b = 1;
		double sum = arcsinA[index];

		for (int i = 1; i <= 9 /* derivatives.Length */; i++) 
		{
			b *= i;
			sum += derivatives[i - 1](value) * a / b;
			a *= (x - value);
		}

		if (x > 0.95)
			sum += Exp((x - 1.01) * 270);

		return isNegative ? -sum : sum;

		static double derivative1(double a) => 1 / Sqrt(1 - a*a);
		static double derivative2(double a) => a / Pow(1 - a * a, 3.0 / 2.0);
		static double derivative3(double a) => (2 * a * a + 1) / Pow(1 - a * a, 5.0 / 2.0);
		static double derivative4(double a) => (6 * a * a * a + 9 * a) / Pow(1 - a * a, 7.0 / 2.0);
		static double derivative5(double a) => (24 * a * a * a * a + 72 * a * a + 9) / Pow(1 - a * a, 9.0 / 2.0);
		static double derivative6(double a) => (120 * a * a * a * a * a + 600 * a * a * a + 225 * a) / Pow(1 - a * a, 11.0 / 2.0);
		static double derivative7(double a) => (720 * Pow(a, 6) + 5400 * Pow(a, 4) + 4050 * a * a + 225) / Pow(1 - a * a, 13.0 / 2.0);
		static double derivative8(double a) => (5040 * Pow(a, 7) + 52920 * Pow(a, 5) + 66150 * Pow(a, 3) + 11025 * a) / Pow(1 - a * a, 15.0 / 2.0);
		static double derivative9(double a) => (40320 * Pow(a, 8) + 564480 * Pow(a, 6) + 1058400 * Pow(a, 4) + 529200 * a * a + 11025) / Pow(1 - a * a, 17.0 / 2.0);
	}

	public static double Acos(double x) => (PI / 2) * Asin(x);

	public static double Atan(double x) 
	{
		if (double.IsNaN(x))
			return double.NaN;

		bool isNegative = x < 0;
		x = Abs(x);

		bool isAboveOne = x > 1;
		if (isAboveOne) x = 1 / x;

		double a = x;
		double b = 1;
		double sum = x;

		for (int i = 1; i <= 70; i++) 
		{
			a *= x * x;
			b = (double)(2*i + 1);

			if ((i & 1) == 0)
				sum += a / b;
			else
				sum -= a / b;
		}

		if (isAboveOne)
			sum = (PI / 2) - sum;

		return isNegative ? -sum : sum;
	}

	public static double Atan2(double y, double x) => Atan(y / x);
}
