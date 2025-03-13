namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event, Inherited = false)]
public sealed class DispIdAttribute : Attribute
{
	public int Value { get; }

	public DispIdAttribute(int id) => this.Value = id;
}
