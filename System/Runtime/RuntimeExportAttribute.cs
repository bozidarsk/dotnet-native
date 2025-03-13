namespace System.Runtime;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
#if EASYINTEROP
public
#else
internal
#endif
sealed class RuntimeExportAttribute : Attribute
{
	public string EntryPoint;

	public RuntimeExportAttribute(string entry) => this.EntryPoint = entry;
}
