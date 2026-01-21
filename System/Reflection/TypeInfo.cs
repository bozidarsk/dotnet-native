using System;
using System.Runtime.CompilerServices;
using Internal;
using Internal.Runtime;
using Internal.NativeFormat;
using Internal.Metadata.NativeFormat;

namespace System.Reflection;

public class TypeInfo : Type
{
	private readonly RuntimeTypeHandle typeHandle;

	public override RuntimeTypeHandle TypeHandle => typeHandle;
	public override string FullName => Name;

	public override string Name 
	{
		get 
		{
			if (!TryFind(out MetadataReader metadata, out Handle handle))
				throw new BadImageFormatException();

			if (handle.HandleType != HandleType.TypeReference)
				throw new BadImageFormatException();

			var typeReference = metadata.GetTypeReference(handle.ToTypeReferenceHandle(metadata));

			return typeReference.TypeName.GetConstantStringValue(metadata).Value;
		}
	}

	private unsafe bool TryFind(out MetadataReader metadata, out Handle handle) 
	{
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
			handle = new Handle((int)token);

			return true;
		}

		handle = default;
		return false;
	}

	[Intrinsic] public static bool operator == (TypeInfo left, TypeInfo right) => RuntimeTypeHandle.ToIntPtr(left.typeHandle) == RuntimeTypeHandle.ToIntPtr(right.typeHandle);
	[Intrinsic] public static bool operator != (TypeInfo left, TypeInfo right) => !(left == right);

	public override bool Equals(object? other) => (other is TypeInfo typeInfo) ? this == typeInfo : false;
	public override int GetHashCode() => typeHandle.GetHashCode();
	public override string ToString() => FullName;

	internal TypeInfo(RuntimeTypeHandle typeHandle) => this.typeHandle = typeHandle;
}
