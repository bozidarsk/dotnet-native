namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class DllImportAttribute : Attribute
{
	public string Value { get; }

	public string? EntryPoint;
	public CharSet CharSet;
	public bool SetLastError;
	public bool ExactSpelling;
	public CallingConvention CallingConvention;
	public bool BestFitMapping;
	public bool PreserveSig;
	public bool ThrowOnUnmappableChar;

	#if EASYINTEROP
	public DllImportAttribute(string name = "*") => this.Value = name;
	#else
	public DllImportAttribute(string name) => this.Value = name;
	#endif
}
