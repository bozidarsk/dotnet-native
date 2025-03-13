namespace System.Collections;

public interface IDictionary : ICollection
{
	object? this[object key] { set; get; }
	ICollection Keys { get; }
	ICollection Values { get; }
	bool IsReadOnly { get; }
	bool IsFixedSize { get; }

	void Add(object key, object? value);
	bool Contains(object key);
	void Clear();
	void Remove(object key);
}
