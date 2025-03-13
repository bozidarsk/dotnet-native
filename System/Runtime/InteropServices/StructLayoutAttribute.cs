namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class StructLayoutAttribute : Attribute
{
	public LayoutKind Value;
	public int Pack;
	public int Size;
	public CharSet CharSet;

	public StructLayoutAttribute(LayoutKind layoutKind) => this.Value = layoutKind;
}
