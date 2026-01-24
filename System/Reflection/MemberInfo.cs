using System;

namespace System.Reflection;

public abstract class MemberInfo 
{
	public abstract MemberTypes MemberType { get; }
	public abstract int MetadataToken { get; }
	public abstract string Name { get; }
}
