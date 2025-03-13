namespace Internal.Runtime;

public enum InterfaceDispatchCellCachePointerFlags 
{
	CachePointerPointsAtCache = 0x0,
	CachePointerIsInterfacePointerOrMetadataToken = 0x1,
	CachePointerIsIndirectedInterfaceRelativePointer = 0x2,
	CachePointerIsInterfaceRelativePointer = 0x3,
	CachePointerMask = 0x3,
	CachePointerMaskShift = 0x2,
	MaxVTableOffsetPlusOne = 0x1000
}
