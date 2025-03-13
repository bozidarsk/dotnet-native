namespace System.Runtime.InteropServices;

public abstract class NativeFunctionPointerWrapper 
{
	public nint NativeFunctionPointer { private set; get; }

	public NativeFunctionPointerWrapper(nint pointer) => this.NativeFunctionPointer = pointer;
}
