using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Internal.Runtime;
using Internal.NativeFormat;
using Internal.Metadata.NativeFormat;

namespace System.Reflection;

public class TypeInfo : Type
{
	private readonly RuntimeTypeHandle typeHandle;

	public override RuntimeTypeHandle TypeHandle => typeHandle;

	public override MemberTypes MemberType => MemberTypes.TypeInfo;

	public override int MetadataToken 
	{
		get 
		{
			if (!typeHandle.GetMetadata(out MetadataReader metadata, out TypeReference typeReference))
				throw new BadImageFormatException();

			return typeReference.Handle.GetHashCode();
		}
	}

	public override string Name 
	{
		get 
		{
			if (!typeHandle.GetMetadata(out MetadataReader metadata, out TypeReference typeReference))
				throw new BadImageFormatException();

			return typeReference.TypeName.GetConstantStringValue(metadata).Value;
		}
	}

	public override string FullName 
	{
		get 
		{
			if (!typeHandle.GetMetadata(out MetadataReader metadata, out TypeReference typeReference))
				throw new BadImageFormatException();

			return resolve(typeReference.ParentNamespaceOrType)! + typeReference.TypeName.GetConstantStringValue(metadata).Value;

			string? resolve(Handle handle) 
			{
				string? name;

				switch (handle.HandleType) 
				{
					case HandleType.NamespaceReference:
						var namespaceReference = metadata.GetNamespaceReference(handle.ToNamespaceReferenceHandle(metadata));
						name = resolve(namespaceReference.ParentScopeOrNamespace);
						if (namespaceReference.Name.IsNull(metadata)) return name;
						else if (name != null) return name + namespaceReference.Name.GetConstantStringValue(metadata).Value + ".";
						else return namespaceReference.Name.GetConstantStringValue(metadata).Value + ".";
					case HandleType.TypeReference:
						var typeReference = metadata.GetTypeReference(handle.ToTypeReferenceHandle(metadata));
						name = resolve(typeReference.ParentNamespaceOrType);
						if (name != null) return name + typeReference.TypeName.GetConstantStringValue(metadata).Value + "+";
						else return typeReference.TypeName.GetConstantStringValue(metadata).Value + "+";
					default:
						return null;
				}
			}
		}
	}

	public override string Namespace 
	{
		get 
		{
			if (!typeHandle.GetMetadata(out MetadataReader metadata, out TypeReference typeReference))
				throw new BadImageFormatException();

			return resolve(typeReference.ParentNamespaceOrType)!;

			string? resolve(Handle handle) 
			{
				string? name;

				switch (handle.HandleType) 
				{
					case HandleType.NamespaceReference:
						var namespaceReference = metadata.GetNamespaceReference(handle.ToNamespaceReferenceHandle(metadata));
						name = resolve(namespaceReference.ParentScopeOrNamespace);
						if (namespaceReference.Name.IsNull(metadata)) return name;
						else if (name != null) return name +  "." + namespaceReference.Name.GetConstantStringValue(metadata).Value;
						else return namespaceReference.Name.GetConstantStringValue(metadata).Value;
					case HandleType.ScopeReference:
						// var scopeReference = metadata.GetScopeReference(handle.ToScopeReferenceHandle(metadata));
						// return scopeReference.Name.GetConstantStringValue(metadata).Value;
					default:
						return null;
				}
			}
		}
	}


	[Intrinsic] public static bool operator == (TypeInfo left, TypeInfo right) => RuntimeTypeHandle.ToIntPtr(left.typeHandle) == RuntimeTypeHandle.ToIntPtr(right.typeHandle);
	[Intrinsic] public static bool operator != (TypeInfo left, TypeInfo right) => !(left == right);

	public override bool Equals(object? other) => (other is TypeInfo typeInfo) ? this == typeInfo : false;
	public override int GetHashCode() => typeHandle.GetHashCode();
	public override string ToString() => FullName;

	internal TypeInfo(RuntimeTypeHandle typeHandle) => this.typeHandle = typeHandle;
}
