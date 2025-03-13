using System.Runtime.CompilerServices;

namespace Internal.Runtime;

internal static class WellKnownEETypes 
{
	internal static unsafe bool IsSystemObject(MethodTable* pEEType) => !(pEEType->IsArray) ? (pEEType->NonArrayBaseType == null) && !pEEType->IsInterface : false;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static unsafe bool IsValidArrayBaseType(MethodTable* pEEType) 
	{
		EETypeElementType elementType = pEEType->ElementType;
		return elementType == EETypeElementType.SystemArray || (elementType == EETypeElementType.Class && pEEType->NonArrayBaseType == null);
	}
}
