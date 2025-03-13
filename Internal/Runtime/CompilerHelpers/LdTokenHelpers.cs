using System;

namespace Internal.Runtime.CompilerHelpers;

internal static class LdTokenHelpers 
{
	private static RuntimeTypeHandle GetRuntimeTypeHandle(nint pEEType) => new RuntimeTypeHandle(new EETypePtr(pEEType));
	private static unsafe Type GetRuntimeType(MethodTable* pMT) => Type.GetTypeFromMethodTable(pMT);

	private static unsafe RuntimeMethodHandle GetRuntimeMethodHandle(nint pHandleSignature) 
	{
		RuntimeMethodHandle returnValue;
		*(nint*)&returnValue = pHandleSignature;
		return returnValue;
	}

	private static unsafe RuntimeFieldHandle GetRuntimeFieldHandle(nint pHandleSignature) 
	{
		RuntimeFieldHandle returnValue;
		*(nint*)&returnValue = pHandleSignature;
		return returnValue;
	}
}
