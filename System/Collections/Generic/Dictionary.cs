using System;
using System.Collections;

namespace System.Collections.Generic;

#pragma warning disable CS8600, CS8601, CS8604

// TODO: implement a hash table
// TODO: implement interfaces
public class Dictionary<TKey, TValue> //:  IDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
	private List<TKey> keys;
	private List<TValue> values;

	public int Count => keys.Count;

	public TValue this[TKey key] 
	{
		set 
		{
			if (key == null)
				throw new ArgumentNullException();

			int index = keys.IndexOf(key);

			if (index != -1) 
			{
				values[index] = value;
				return;
			}

			keys.Add(key);
			values.Add(value);
		}
		get 
		{
			if (key == null)
				throw new ArgumentNullException();

			int index = keys.IndexOf(key);
			return values[(index == -1) ? throw new KeyNotFoundException() : index];
		}
	}

	public void Clear() 
	{
		keys.Clear();
		values.Clear();
	}

	public bool ContainsKey(TKey key) => keys.Contains(key);
	public bool ContainsValue(TValue value) => values.Contains(value);

	public bool Remove(TKey key) => Remove(key, out TValue value);
	public bool Remove(TKey key, out TValue value) 
	{
		if (key == null)
			throw new ArgumentNullException();

		int index = keys.IndexOf(key);

		if (index == -1) 
		{
			value = default;
			return false;
		}

		value = values[index];

		keys.RemoveAt(index);
		values.RemoveAt(index);

		return true;
	}

	public void Add(TKey key, TValue value) 
	{
		if (!TryAdd(key, value))
			throw new ArgumentException();
	}

	public bool TryAdd(TKey key, TValue value) 
	{
		if (key == null)
			throw new ArgumentNullException();

		int index = keys.IndexOf(key);

		if (index != -1)
			return false;

		keys.Add(key);
		values.Add(value);
		return true;
	}

	public bool TryGetValue(TKey key, out TValue value) 
	{
		if (key == null)
			throw new ArgumentNullException();

		int index = keys.IndexOf(key);

		if (index == -1) 
		{
			value = default;
			return false;
		}

		value = values[index];
		return true;
	}

	public Dictionary(int capacity = 4) 
	{
		this.keys = new(capacity);
		this.values = new(capacity);
	}

	public Dictionary(IDictionary<TKey, TValue> data) : this()
	{
		if (data == null)
			throw new ArgumentNullException();

		if (data.Keys.Count != data.Values.Count)
			throw new ArgumentException("The count of keys and values must be the same.");

		foreach (var key in data.Keys)
			this.keys.Add(key);

		foreach (var value in data.Values)
			this.values.Add(value);
	}

	public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> data) : this()
	{
		if (data == null)
			throw new ArgumentNullException();

		foreach (var x in data) 
		{
			this.keys.Add(x.Key);
			this.values.Add(x.Value);
		}
	}
}
