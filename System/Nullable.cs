namespace System;

public partial struct Nullable<T> where T : struct
{
	private readonly bool hasValue;
	private T value;

	public readonly bool HasValue => hasValue;
	public readonly T Value => !hasValue ? throw new InvalidOperationException("Nullable object must have a value.") : value;

	public readonly T GetValueOrDefault() => value;
	public readonly T GetValueOrDefault(T defaultValue) => hasValue ? value : defaultValue;

	public override int GetHashCode() => hasValue ? value.GetHashCode() : 0;
	public override string ToString() => hasValue ? value.ToString() : "";

	public override bool Equals(object? other) 
	{
		if (!hasValue)
			return other == null;

		return (other != null) ? value.Equals(other) : false;
	}

	public static implicit operator T?(T value) => new T?(value);
	public static explicit operator T(T? value) => value!.Value;

	public Nullable(T value) 
	{
		this.value = value;
		hasValue = true;
	}
}
