namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LibraryImportAttribute : Attribute
{
	public string LibraryName { get; }
	public string? EntryPoint { set; get; }
	public StringMarshalling StringMarshalling { set; get; }
	public Type? StringMarshallingCustomType { set; get; }
	public bool SetLastError { set; get; }

	public LibraryImportAttribute(string name) => this.LibraryName = name;
}
