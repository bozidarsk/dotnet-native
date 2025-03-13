namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.ReturnValue, Inherited = false)]
public sealed partial class MarshalAsAttribute : Attribute
{
	public MarshalAsAttribute(UnmanagedType unmanagedType) => Value = unmanagedType;
	public MarshalAsAttribute(short unmanagedType) => Value = (UnmanagedType)unmanagedType;

	public UnmanagedType Value { get; }
	public VarEnum SafeArraySubType;
	public Type? SafeArrayUserDefinedSubType;
	public int IidParameterIndex;
	public UnmanagedType ArraySubType;
	public short SizeParamIndex;
	public int SizeConst;
	public string? MarshalType;
	public Type? MarshalTypeRef;
	public string? MarshalCookie;
}
