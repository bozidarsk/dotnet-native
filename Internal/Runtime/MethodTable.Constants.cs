using System;

namespace Internal.Runtime;

[Flags]
internal enum EETypeFlags : uint
{
	EETypeKindMask = 0x00030000,
	HasDispatchMap = 0x00040000,
	IsDynamicTypeFlag = 0x00080000,
	HasFinalizerFlag = 0x00100000,
	HasPointersFlag = 0x00200000,
	HasSealedVTableEntriesFlag = 0x00400000,
	GenericVarianceFlag = 0x00800000,
	OptionalFieldsFlag = 0x01000000,
	IsGenericFlag = 0x02000000,
	ElementTypeMask = 0x7C000000,
	ElementTypeShift = 26,
	HasComponentSizeFlag = 0x80000000,
}

[Flags]
internal enum EETypeFlagsEx : ushort
{
	HasEagerFinalizerFlag = 0x0001,
	HasCriticalFinalizerFlag = 0x0002,
	IsTrackedReferenceWithFinalizerFlag = 0x0004,
	IDynamicInterfaceCastableFlag = 0x0008,
}

internal enum EETypeKind : uint
{
	CanonicalEEType = 0x00000000,
	FunctionPointerEEType = 0x00010000,
	ParameterizedEEType = 0x00020000,
	GenericTypeDefEEType = 0x00030000,
}

[Flags]
internal enum EETypeRareFlags : int
{
	RequiresAlign8Flag = 0x00000001,
	HasCctorFlag = 0x0000020,
	IsHFAFlag = 0x00000100,
	IsDynamicTypeWithGcStatics = 0x00000400,
	IsDynamicTypeWithNonGcStatics = 0x00000800,
	IsDynamicTypeWithThreadStatics = 0x00001000,
	IsByRefLikeFlag = 0x00008000,
}

internal enum EETypeField 
{
	ETF_TypeManagerIndirection,
	ETF_WritableData,
	ETF_DispatchMap,
	ETF_Finalizer,
	ETF_OptionalFieldsPtr,
	ETF_SealedVirtualSlots,
	ETF_DynamicTemplateType,
	ETF_GenericDefinition,
	ETF_GenericComposition,
	ETF_FunctionPointerParameters,
	ETF_DynamicGcStatics,
	ETF_DynamicNonGcStatics,
	ETF_DynamicThreadStaticOffset,
}

internal enum EETypeElementType 
{
	Unknown = 0x00,
	Void = 0x01,
	Boolean = 0x02,
	Char = 0x03,
	SByte = 0x04,
	Byte = 0x05,
	Int16 = 0x06,
	UInt16 = 0x07,
	Int32 = 0x08,
	UInt32 = 0x09,
	Int64 = 0x0A,
	UInt64 = 0x0B,
	IntPtr = 0x0C,
	UIntPtr = 0x0D,
	Single = 0x0E,
	Double = 0x0F,

	ValueType = 0x10,
	Nullable = 0x12,

	Class = 0x14,
	Interface = 0x15,

	SystemArray = 0x16,

	Array = 0x17,
	SzArray = 0x18,
	ByRef = 0x19,
	Pointer = 0x1A,
	FunctionPointer = 0x1B,
}

internal enum EETypeOptionalFieldTag : byte
{
	RareFlags,
	ValueTypeFieldPadding,
	NullableValueOffset,
	Count,
}

internal enum GenericVariance : byte
{
	NonVariant = 0,
	Covariant = 1,
	Contravariant = 2,
	ArrayCovariant = 0x20,
}

internal static class ParameterizedTypeShapeConstants 
{
	public const int Pointer = 0;
	public const int ByRef = 1;
}

internal static class FunctionPointerFlags 
{
	public const uint IsUnmanaged = 0x80000000;
	public const uint FlagsMask = IsUnmanaged;
}

internal static class StringComponentSize 
{
	public const int Value = sizeof(char);
}

internal static class WritableData 
{
	public static int GetSize(int pointerSize) => pointerSize;
	public static int GetAlignment(int pointerSize) => pointerSize;
}
