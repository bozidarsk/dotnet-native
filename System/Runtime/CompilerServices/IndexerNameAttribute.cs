namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class IndexerNameAttribute : Attribute
{
	public IndexerNameAttribute(string name) {}
}
