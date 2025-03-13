using System;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Internal.Runtime;

internal unsafe partial struct MethodTable 
{
	internal static bool AreSameType(MethodTable* mt1, MethodTable* mt2) => (mt1 == mt2) ? true : mt1->IsEquivalentTo(mt2);
	internal nint GetClasslibFunction(ClassLibFunctionId id) => (nint)InternalCalls.RhpGetClasslibFunctionFromEEType((MethodTable*)Unsafe.AsPointer(ref this), id);
	internal Exception GetClasslibException(ExceptionIDs id) 
	{
		switch (id) 
		{
			case ExceptionIDs.OutOfMemory:
				return new OutOfMemoryException();
			case ExceptionIDs.Arithmetic:
				return new ArithmeticException();
			case ExceptionIDs.ArrayTypeMismatch:
				return new ArrayTypeMismatchException();
			case ExceptionIDs.DivideByZero:
				return new DivideByZeroException();
			case ExceptionIDs.IndexOutOfRange:
				return new IndexOutOfRangeException();
			case ExceptionIDs.InvalidCast:
				return new InvalidCastException();
			case ExceptionIDs.Overflow:
				return new OverflowException();
			case ExceptionIDs.NullReference:
				return new NullReferenceException();
			case ExceptionIDs.AccessViolation:
				return new AccessViolationException();
			case ExceptionIDs.DataMisaligned:
				return new DataMisalignedException();
			case ExceptionIDs.EntrypointNotFound:
				return new EntryPointNotFoundException();
			case ExceptionIDs.AmbiguousImplementation:
				return new AmbiguousImplementationException();
			default:
				Environment.FailFast("The runtime requires an exception for a case that this class library does not understand.");
				return null;
		}
	}

	internal bool IsEquivalentTo(MethodTable* pOtherEEType) 
	{
		fixed (MethodTable* pThis = &this) 
		{
			if (pThis == pOtherEEType)
				return true;

			MethodTable* pThisEEType = pThis;

			if (pThisEEType == pOtherEEType)
				return true;

			if (pThisEEType->IsParameterizedType && pOtherEEType->IsParameterizedType)
			{
				return 
					pThisEEType->RelatedParameterType->IsEquivalentTo(pOtherEEType->RelatedParameterType) &&
					pThisEEType->ParameterizedTypeShape == pOtherEEType->ParameterizedTypeShape
				;
			}
		}

		return false;
	}
}
