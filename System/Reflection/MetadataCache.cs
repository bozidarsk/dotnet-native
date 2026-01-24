using System.Collections.Generic;
using Internal.Runtime;
using Internal.NativeFormat;
using Internal.Metadata.NativeFormat;

namespace System.Reflection;

internal static unsafe class MetadataCache 
{
	private static Dictionary<RuntimeTypeHandle, (MetadataReader, TypeReference)> typeReferences = new();

	public static bool GetMetadata(this RuntimeTypeHandle typeHandle, out MetadataReader metadata, out TypeReference typeReference) 
	{
		if (typeReferences.TryGetValue(typeHandle, out (MetadataReader, TypeReference) value)) 
		{
			(metadata, typeReference) = value;
			return true;
		}

		typeReference = default;

		MethodTable* mt = typeHandle.ToMethodTable();
		var tm = mt->TypeManager.AsTypeManager();

		nint metadataBlob = tm->GetModuleSection((ReadyToRunSectionType)(300 + ReflectionMapBlob.EmbeddedMetadata), out int metadataLength);
		nint typeMapBlob = tm->GetModuleSection((ReadyToRunSectionType)(300 + ReflectionMapBlob.TypeMap), out int typeMapLength);
		nint fixupsTableBlob = tm->GetModuleSection((ReadyToRunSectionType)(300 + ReflectionMapBlob.CommonFixupsTable), out int fixupsTableLength);

		metadata = new MetadataReader(metadataBlob, metadataLength);
		var typeMap = new NativeReader((byte*)typeMapBlob, (uint)typeMapLength);
		var typeHash = new NativeHashtable(new NativeParser(typeMap, 0));

		var allTypes = typeHash.EnumerateAllEntries();
		NativeParser typeParser;

		while (!(typeParser = allTypes.GetNext()).IsNull) 
		{
			uint index = typeParser.GetUnsigned();

			if (index >= fixupsTableLength / sizeof(uint))
				throw new IndexOutOfRangeException();

			var foundMt = MethodTable.SupportsRelativePointers ? (nint)RH.ReadRelPtr32(&((int*)fixupsTableBlob)[index]) : (nint)(((void**)fixupsTableBlob)[index]);

			if ((nint)mt != foundMt)
				continue;

			var token = typeParser.GetUnsigned();
			var handle = new Handle((int)token);

			if (handle.HandleType != HandleType.TypeReference)
				return false;

			typeReference = metadata.GetTypeReference(handle.ToTypeReferenceHandle(metadata));

			typeReferences[typeHandle] = (metadata, typeReference);
			return true;
		}

		return false;
	}
}
