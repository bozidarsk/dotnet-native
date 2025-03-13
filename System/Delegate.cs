namespace System;

public abstract class Delegate 
{
	internal object? m_firstParameter;
	internal object? m_helperObject;
	internal nint m_extraFunctionPointerOrData;
	internal nint m_functionPointer;

	private protected const int MulticastThunk = 0;
	private protected const int ClosedStaticThunk = 1;
	private protected const int OpenStaticThunk = 2;
	private protected const int ClosedInstanceThunkOverGenericMethod = 3;
	private protected const int OpenInstanceThunk = 4;
	private protected const int ObjectArrayThunk = 5;

	private void InitializeClosedStaticThunk(object firstParameter, nint functionPointer, nint functionPointerThunk) 
	{
		this.m_extraFunctionPointerOrData = functionPointer;
		this.m_helperObject = firstParameter;
		this.m_functionPointer = functionPointerThunk;
		this.m_firstParameter = this;
	}

	private void InitializeOpenStaticThunk(object _ /*firstParameter*/, nint functionPointer, nint functionPointerThunk) 
	{
		this.m_firstParameter = this;
		this.m_functionPointer = functionPointerThunk;
		this.m_extraFunctionPointerOrData = functionPointer;
	}

	private void InitializeClosedInstance(object? firstParameter, nint functionPointer) 
	{
		this.m_firstParameter = firstParameter;
		this.m_functionPointer = functionPointer;
	}
}
