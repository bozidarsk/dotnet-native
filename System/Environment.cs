using System.Runtime;

namespace System;

public static class Environment 
{
	public static void FailFast(string message) => FailFast(message, null);
	public static void FailFast(string message, Exception? exception) 
	{
		Console.WriteLine(message);

		if (exception != null && exception.Message != null)
			Console.WriteLine(exception.Message);

		RH.RhpFallbackFailFast(message, exception);
	}
}
