using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Internal.Runtime;

public unsafe partial struct TypeManagerHandle : IEquatable<TypeManagerHandle>
{
	private TypeManager* handleValue;

	public bool IsNull => handleValue == null;
	public unsafe nint OsModuleBase => handleValue->OsHandle;

	public nint GetIntPtrUNSAFE() => (nint)handleValue;
	internal unsafe TypeManager* AsTypeManager() => handleValue;

	public static bool operator == (TypeManagerHandle left, TypeManagerHandle right) => left.handleValue == right.handleValue;
	public static bool operator != (TypeManagerHandle left, TypeManagerHandle right) => left.handleValue != right.handleValue;

	public bool Equals(TypeManagerHandle other) => this.handleValue == other.handleValue;

	public override bool Equals(object? o) => (o is TypeManagerHandle) ? this.handleValue == ((TypeManagerHandle)o).handleValue : false;
	public override int GetHashCode() => ((nint)handleValue).GetHashCode();
	public override string ToString() => ((nint)handleValue).ToString();

	public TypeManagerHandle(nint handleValue) => this.handleValue = (TypeManager*)handleValue;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct TypeManager 
{
	public nint OsHandle;
	public ReadyToRunHeader* ReadyToRunHeader;
	private byte* pStaticsGCDataSection;
	private byte* pThreadStaticsDataSection;
	private void** pClasslibFunctions;
	private uint nClasslibFunctions;

	public nint GetModuleSection(ReadyToRunSectionType sectionId, int* length) 
	{
		ModuleInfoRow* pModuleInfoRows = (ModuleInfoRow*)(ReadyToRunHeader + 1);

		for (int i = 0; i < ReadyToRunHeader->NumberOfSections; i++) 
		{
			ModuleInfoRow* pCurrent = pModuleInfoRows + i;
			if ((int)sectionId == pCurrent->SectionId) 
			{
				*length = pCurrent->Length;
				return pCurrent->Start;
			}
		}

		*length = 0;
		return 0;
	}

	public void* GetClasslibFunction(ClassLibFunctionId functionId) 
	{
		uint id = (uint)functionId;

		if (id >= nClasslibFunctions)
			return (void*)0;

		return pClasslibFunctions[id];
	}
}
