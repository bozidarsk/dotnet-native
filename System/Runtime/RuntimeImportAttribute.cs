namespace System.Runtime;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
internal sealed class RuntimeImportAttribute : Attribute
{
	public string DllName { get; }
	public string EntryPoint { get; }

	public RuntimeImportAttribute(string entry) => this.EntryPoint = entry;
	public RuntimeImportAttribute(string dllName, string entry) : this(entry) => this.DllName = dllName;
}
