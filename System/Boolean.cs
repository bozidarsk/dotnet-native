namespace System;

public readonly struct Boolean 
{
	public const string TrueString = "True";
	public const string FalseString = "False";

	public static bool Parse(string? str) => TryParse(str, out bool result) ? result : throw new FormatException();
	public static unsafe bool TryParse(string? str, out bool result) 
	{
		result = false;

		if (str == null || str.Length == 0)
			return false;

		if (str.ToLower() == TrueString.ToLower()) 
		{
			result = true;
			return true;
		}

		if (str.ToLower() == FalseString.ToLower()) 
		{
			result = false;
			return true;
		}

		return false;
	}

	public override int GetHashCode() => this ? 1 : 0;
	public override string ToString() => this ? TrueString : FalseString;
}
