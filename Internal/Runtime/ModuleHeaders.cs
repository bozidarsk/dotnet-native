using System;
using System.Runtime.InteropServices;

namespace Internal.Runtime;

internal struct ReadyToRunHeaderConstants 
{
	public const uint Signature = 0x00525452;

	public const ushort CurrentMajorVersion = 9;
	public const ushort CurrentMinorVersion = 1;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ReadyToRunHeader 
{
	public uint Signature;
	public ushort MajorVersion;
	public ushort MinorVersion;

	public ReadyToRunHeaderFlags Flags;

	public ushort NumberOfSections;
	public byte EntrySize;
	public byte EntryType;
}

[StructLayout(LayoutKind.Sequential)]
internal struct ModuleInfoRow 
{
	public ReadyToRunSectionType SectionId;
	public ModuleInfoFlags Flags;
	public IntPtr Start;
	public IntPtr End;

	public bool HasEndPointer => !End.Equals(IntPtr.Zero);
	public int Length => (int)((ulong)End - (ulong)Start);
}

[Flags]
public enum ReadyToRunHeaderFlags : uint
{
	PlatformNeutralSource = 0x00000001,
	Composite = 0x00000002,
	Partial = 0x00000004,
	NonsharedPinvokeStubs = 0x00000008,
	EmbeddedMsil = 0x00000010,
	Component = 0x00000020,
	MultimoduleVersionBubble = 0x00000040,
	UnrelatedR2rCode = 0x00000080,
}

public enum ReadyToRunSectionType : int
{
	CompilerIdentifier = 100,
	ImportSections = 101,
	RuntimeFunctions = 102,
	MethodDefEntryPoints = 103,
	ExceptionInfo = 104,
	DebugInfo = 105,
	DelayLoadMethodCallThunks = 106,
	AvailableTypes = 108,
	InstanceMethodEntryPoints = 109,
	InliningInfo = 110,
	ProfileDataInfo = 111,
	ManifestMetadata = 112,
	AttributePresence = 113,
	InliningInfo2 = 114,
	ComponentAssemblies = 115,
	OwnerCompositeExecutable = 116,
	PgoInstrumentationData = 117,
	ManifestAssemblyMvids = 118,
	CrossModuleInlineInfo = 119,
	HotColdMap = 120,
	MethodIsGenericMap = 121,
	EnclosingTypeMap = 122,
	TypeGenericInfoMap = 123,

	StringTable = 200,
	GCStaticRegion = 201,
	ThreadStaticRegion = 202,
	TypeManagerIndirection = 204,
	EagerCctor = 205,
	FrozenObjectRegion = 206,
	DehydratedData = 207,
	ThreadStaticOffsetRegion = 208,
	ImportAddressTables = 212,
	ModuleInitializerList = 213,

	ReadonlyBlobRegionStart = 300,
	ReadonlyBlobRegionEnd = 399,
}

[Flags]
internal enum ModuleInfoFlags : int
{
	HasEndPointer = 0x1,
}
