using System.Collections.Generic;

#pragma warning disable CS8602

namespace System.Linq;

public static partial class Enumerable 
{
	public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func) 
	{
		if (source == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

		if (func == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);

		using (IEnumerator<TSource> e = source.GetEnumerator()) 
		{
			if (!e.MoveNext())
				ThrowHelper.ThrowNoElementsException();

			TSource? result = e.Current;

			while (e.MoveNext())
				result = func(result, e.Current);

			return result;
		}
	}

	public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func) 
	{
		if (source == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

		if (func == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);

		TAccumulate result = seed;

		foreach (var x in source)
			result = func(result, x);

		return result;
	}

	public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) 
	{
		if (source == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);

		if (func == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.func);

		if (resultSelector == null)
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.resultSelector);

		TAccumulate result = seed;

		foreach (var x in source)
			result = func(result, x);

		return resultSelector(result);
	}
}
