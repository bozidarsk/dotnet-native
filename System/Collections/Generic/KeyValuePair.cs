namespace System.Collections.Generic;

public static class KeyValuePair 
{
	public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value) =>  new KeyValuePair<TKey, TValue>(key, value);

	internal static string PairToString(object? key, object? value) => $"[{key}, {value}]";
}

public readonly struct KeyValuePair<TKey, TValue> 
{
	private readonly TKey key;
	private readonly TValue value;

	public TKey Key => key;
	public TValue Value => value;

	public void Deconstruct(out TKey key, out TValue value) => (key, value) = (this.key, this.value);

	public override string ToString() => KeyValuePair.PairToString(Key, Value);

	public KeyValuePair(TKey key, TValue value) => (this.key, this.value) = (key, value);
}
