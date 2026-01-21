using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Internal.Runtime;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct TypeManagerSlot 
{
	public TypeManagerHandle TypeManager;
	public int ModuleIndex;
}

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
	private nint* pClasslibFunctions;
	private uint nClasslibFunctions;

	public nint GetModuleSection(ReadyToRunSectionType sectionId, out int length) 
	{
		ModuleInfoRow* rows = (ModuleInfoRow*)(this.ReadyToRunHeader + 1);

		for (int i = 0; i < this.ReadyToRunHeader->NumberOfSections; i++) 
		{
			if (sectionId == rows[i].SectionId) 
			{
				length = rows[i].Length;
				return rows[i].Start;
			}
		}

		length = 0;
		return 0;
	}

	public void* GetClasslibFunction(ClassLibFunctionId functionId) 
	{
		uint id = (uint)functionId;

		if (id >= nClasslibFunctions)
			return (void*)0;

		return (void*)pClasslibFunctions[id];
	}

	internal TypeManager(nint osModule, ReadyToRunHeader* pModuleHeader, nint* pClasslibFunctions, uint nClasslibFunctions) 
	{
		this.ReadyToRunHeader = pModuleHeader;

		if (
			ReadyToRunHeader->Signature != ReadyToRunHeaderConstants.Signature
			|| ReadyToRunHeader->MajorVersion != ReadyToRunHeaderConstants.CurrentMajorVersion
			|| ReadyToRunHeader->MinorVersion != ReadyToRunHeaderConstants.CurrentMinorVersion
		) Environment.FailFast("ReadyToRunHeader with invalid constants.");

		this.OsHandle = osModule;
		this.pClasslibFunctions = pClasslibFunctions;
		this.nClasslibFunctions = nClasslibFunctions;

		int length;
		this.pStaticsGCDataSection = (byte*)GetModuleSection(ReadyToRunSectionType.GCStaticRegion, out length);
		this.pThreadStaticsDataSection = (byte*)GetModuleSection(ReadyToRunSectionType.ThreadStaticRegion, out length);
	}
}
