namespace Internal.Runtime;

internal static class IndirectionConstants 
{
	public const int IndirectionCellPointer = 0x1;
	public const uint RVAPointsToIndirection = 0x80000000u;
}

internal static class GCStaticRegionConstants 
{
	public const int Uninitialized = 0x1;
	public const int HasPreInitializedData = 0x2;
	public const int Mask = Uninitialized | HasPreInitializedData;
}

internal static class ArrayTypesConstants 
{
	public const int MaxSizeForValueClassInArray = 0xFFFF;
}

internal enum GC_ALLOC_FLAGS 
{
	GC_ALLOC_NO_FLAGS = 0,
	GC_ALLOC_ZEROING_OPTIONAL = 16,
	GC_ALLOC_PINNED_OBJECT_HEAP = 64,
}

internal static class SpecialDispatchMapSlot 
{
	public const ushort Diamond = 0xFFFE;
	public const ushort Reabstraction = 0xFFFF;
}

internal static class SpecialGVMInterfaceEntry 
{
	public const uint Diamond = 0xFFFFFFFF;
	public const uint Reabstraction = 0xFFFFFFFE;
}

internal static class StaticVirtualMethodContextSource 
{
	public const ushort None = 0;
	public const ushort ContextFromThisClass = 1;
	public const ushort ContextFromFirstInterface = 2;
}

internal enum RuntimeHelperKind 
{
	AllocateObject,
	IsInst,
	CastClass,
	AllocateArray,
}

internal static class MethodFixupCellFlagsConstants 
{
	public const int CharSetMask = 0x7;
	public const int IsObjectiveCMessageSendMask = 0x8;
	public const int ObjectiveCMessageSendFunctionMask = 0x70;
	public const int ObjectiveCMessageSendFunctionShift = 4;
}
