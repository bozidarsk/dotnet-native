namespace System.Linq;

internal static class ThrowHelper 
{
	internal static void ThrowArgumentNullException(ExceptionArgument argument) => throw new ArgumentNullException(GetArgumentString(argument));
	internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument) => throw new ArgumentOutOfRangeException(GetArgumentString(argument));
	internal static void ThrowMoreThanOneElementException() => throw new InvalidOperationException("Sequence contains more than one element.");
	internal static void ThrowMoreThanOneMatchException() => throw new InvalidOperationException("Sequence contains more than one matching element.");
	internal static void ThrowNoElementsException() => throw new InvalidOperationException("Sequence contains no elements.");
	internal static void ThrowNoMatchException() => throw new InvalidOperationException("Sequence contains no matching element.");
	internal static void ThrowNotSupportedException() => throw new NotSupportedException();
	internal static bool ThrowNotSupportedException_Boolean() => throw new NotSupportedException();
	internal static void ThrowOverflowException() => throw new OverflowException();

	private static string GetArgumentString(ExceptionArgument argument) => argument switch 
	{
		ExceptionArgument.collectionSelector => nameof(ExceptionArgument.collectionSelector),
		ExceptionArgument.count => nameof(ExceptionArgument.count),
		ExceptionArgument.elementSelector => nameof(ExceptionArgument.elementSelector),
		ExceptionArgument.enumerable => nameof(ExceptionArgument.enumerable),
		ExceptionArgument.first => nameof(ExceptionArgument.first),
		ExceptionArgument.func => nameof(ExceptionArgument.func),
		ExceptionArgument.index => nameof(ExceptionArgument.index),
		ExceptionArgument.inner => nameof(ExceptionArgument.inner),
		ExceptionArgument.innerKeySelector => nameof(ExceptionArgument.innerKeySelector),
		ExceptionArgument.keySelector => nameof(ExceptionArgument.keySelector),
		ExceptionArgument.outer => nameof(ExceptionArgument.outer),
		ExceptionArgument.outerKeySelector => nameof(ExceptionArgument.outerKeySelector),
		ExceptionArgument.predicate => nameof(ExceptionArgument.predicate),
		ExceptionArgument.resultSelector => nameof(ExceptionArgument.resultSelector),
		ExceptionArgument.second => nameof(ExceptionArgument.second),
		ExceptionArgument.selector => nameof(ExceptionArgument.selector),
		ExceptionArgument.source => nameof(ExceptionArgument.source),
		ExceptionArgument.third => nameof(ExceptionArgument.third),
		ExceptionArgument.size => nameof(ExceptionArgument.size),
		_ => string.Empty
	};
}

internal enum ExceptionArgument
{
	collectionSelector,
	count,
	elementSelector,
	enumerable,
	first,
	func,
	index,
	inner,
	innerKeySelector,
	keySelector,
	outer,
	outerKeySelector,
	predicate,
	resultSelector,
	second,
	selector,
	source,
	third,
	size
}
