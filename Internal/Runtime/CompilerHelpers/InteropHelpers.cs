using System;
using System.Runtime.InteropServices;

namespace Internal.Runtime.CompilerHelpers;

internal static class InteropHelpers 
{
	public static nint GetCurrentCalleeOpenStaticDelegateFunctionPointer() => throw new NotImplementedException();//PInvokeMarshal.GetCurrentCalleeOpenStaticDelegateFunctionPointer();
	public static T GetCurrentCalleeDelegate<T>() where T : class => throw new NotImplementedException();//PInvokeMarshal.GetCurrentCalleeDelegate<T>();
}
