namespace System.Collections.Generic;

public interface IList<T> : ICollection<T>
{
	T this[int index] { set; get; }

	int IndexOf(T item);
	void Insert(int index, T item);
	void RemoveAt(int index);
}
