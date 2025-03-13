using System.Runtime;

namespace System;

public static class Environment 
{
	public static void FailFast(string message) => FailFast(message, null);
	public static void FailFast(string message, Exception? exception) => InternalCalls.RhpFallbackFailFast(message, exception);
}
