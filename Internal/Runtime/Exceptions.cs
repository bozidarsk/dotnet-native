using System;

namespace System.Runtime;

public class AmbiguousImplementationException : SystemException
{
	public AmbiguousImplementationException() : base("Ambiguous implementation found.") {}
	public AmbiguousImplementationException(string? message) : base(message) {}
	public AmbiguousImplementationException(string? message, Exception? innerException) : base(message, innerException) {}
}
