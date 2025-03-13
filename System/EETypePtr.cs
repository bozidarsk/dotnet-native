using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using MethodTable = Internal.Runtime.MethodTable;
using MethodTableList = Internal.Runtime.MethodTableList;
using EETypeElementType = Internal.Runtime.EETypeElementType;
using CorElementType = System.Reflection.CorElementType;

namespace System;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EETypePtr : IEquatable<EETypePtr>
{
	private MethodTable* value;

	public EETypePtr(IntPtr value) => this.value = (MethodTable*)value;
	internal EETypePtr(MethodTable* value) => this.value = value;

	internal MethodTable* ToPointer() => value;

	public override bool Equals(object? obj) 
	{
		if (obj is EETypePtr)
		{
			return this == (EETypePtr)obj;
		}
		return false;
	}

	public bool Equals(EETypePtr p) => this == p;
	public static bool operator == (EETypePtr value1, EETypePtr value2) => value1.value == value2.value;
	public static bool operator != (EETypePtr value1, EETypePtr value2) => !(value1 == value2);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => (int)value->HashCode;

	// Caution: You cannot safely compare RawValue's as RH does NOT unify EETypes. Use the == or Equals() methods exposed by EETypePtr itself.
	internal IntPtr RawValue => (IntPtr)value;
	internal bool IsNull => value == null;
	internal bool IsArray => value->IsArray;
	internal bool IsSzArray => value->IsSzArray;
	internal bool IsPointer => value->IsPointerType;
	internal bool IsFunctionPointer => value->IsFunctionPointerType;
	internal bool IsByRef => value->IsByRefType;
	internal bool IsValueType => value->IsValueType;
	internal bool IsString => value->IsString;
	// Warning! UNLIKE the similarly named Reflection api, this method also returns "true" for Enums.
	internal bool IsPrimitive => value->IsPrimitive;
	// WARNING: Never call unless the MethodTable came from an instanced object. Nested enums can be open generics (typeof(Outer<>).NestedEnum)
	// and this helper has undefined behavior when passed such as a enum.

	internal bool IsEnum 
	{
		get
		{
			// Q: When is an enum type a constructed generic type?
			// A: When it's nested inside a generic type.
			if (!(IsDefType))
				return false;

			// Generic type definitions that return true for IsPrimitive are type definitions of generic enums.
			// Otherwise check the base type.
			return (IsGenericTypeDefinition && IsPrimitive) || this.BaseType == EETypePtr.EETypePtrOf<Enum>();
		}
	}

	internal bool IsGenericTypeDefinition => value->IsGenericTypeDefinition;
	internal bool IsGeneric => value->IsGeneric;
	internal GenericArgumentCollection Instantiation => new GenericArgumentCollection(value->GenericArity, value->GenericArguments);
	internal EETypePtr GenericDefinition => new EETypePtr(value->GenericDefinition);
	internal bool IsDefType => !value->IsParameterizedType && !value->IsFunctionPointerType;
	internal bool IsDynamicType => value->IsDynamicType;
	internal bool IsInterface => value->IsInterface;
	internal bool IsByRefLike => value->IsByRefLike;
	internal bool IsNullable => value->IsNullable;
	internal bool HasCctor => value->HasCctor;
	internal bool IsTrackedReferenceWithFinalizer => value->IsTrackedReferenceWithFinalizer;
	internal EETypePtr NullableType => new EETypePtr(value->NullableType);
	internal EETypePtr ArrayElementType => new EETypePtr(value->RelatedParameterType);
	internal int ArrayRank => value->ArrayRank;
	internal InterfaceCollection Interfaces => new InterfaceCollection(value);

	internal EETypePtr BaseType 
	{
		get
		{
			if (IsArray)
				return EETypePtr.EETypePtrOf<Array>();

			if (IsPointer || IsByRef || IsFunctionPointer)
				return new EETypePtr(default(IntPtr));

			EETypePtr baseEEType = new EETypePtr(value->NonArrayBaseType);
			return baseEEType;
		}
	}

	internal IntPtr DispatchMap => (IntPtr)value->DispatchMap;
	// Instance contains pointers to managed objects.
	internal bool ContainsGCPointers => value->ContainsGCPointers;
	internal uint ValueTypeSize => value->ValueTypeSize;

	internal CorElementType CorElementType 
	{
		get
		{
			byte* map = stackalloc byte[32] 
			{
				default,
				(byte)CorElementType.ELEMENT_TYPE_VOID, // EETypeElementType.Void
				(byte)CorElementType.ELEMENT_TYPE_BOOLEAN, // EETypeElementType.Boolean
				(byte)CorElementType.ELEMENT_TYPE_CHAR, // EETypeElementType.Char
				(byte)CorElementType.ELEMENT_TYPE_I1, // EETypeElementType.SByte
				(byte)CorElementType.ELEMENT_TYPE_U1, // EETypeElementType.Byte
				(byte)CorElementType.ELEMENT_TYPE_I2, // EETypeElementType.Int16
				(byte)CorElementType.ELEMENT_TYPE_U2, // EETypeElementType.UInt16
				(byte)CorElementType.ELEMENT_TYPE_I4, // EETypeElementType.Int32
				(byte)CorElementType.ELEMENT_TYPE_U4, // EETypeElementType.UInt32
				(byte)CorElementType.ELEMENT_TYPE_I8, // EETypeElementType.Int64
				(byte)CorElementType.ELEMENT_TYPE_U8, // EETypeElementType.UInt64
				(byte)CorElementType.ELEMENT_TYPE_I, // EETypeElementType.IntPtr
				(byte)CorElementType.ELEMENT_TYPE_U, // EETypeElementType.UIntPtr
				(byte)CorElementType.ELEMENT_TYPE_R4, // EETypeElementType.Single
				(byte)CorElementType.ELEMENT_TYPE_R8, // EETypeElementType.Double

				(byte)CorElementType.ELEMENT_TYPE_VALUETYPE, // EETypeElementType.ValueType
				(byte)CorElementType.ELEMENT_TYPE_VALUETYPE,
				(byte)CorElementType.ELEMENT_TYPE_VALUETYPE, // EETypeElementType.Nullable
				(byte)CorElementType.ELEMENT_TYPE_VALUETYPE,
				(byte)CorElementType.ELEMENT_TYPE_CLASS, // EETypeElementType.Class
				(byte)CorElementType.ELEMENT_TYPE_CLASS, // EETypeElementType.Interface
				(byte)CorElementType.ELEMENT_TYPE_CLASS, // EETypeElementType.SystemArray
				(byte)CorElementType.ELEMENT_TYPE_ARRAY, // EETypeElementType.Array
				(byte)CorElementType.ELEMENT_TYPE_SZARRAY, // EETypeElementType.SzArray
				(byte)CorElementType.ELEMENT_TYPE_BYREF, // EETypeElementType.ByRef
				(byte)CorElementType.ELEMENT_TYPE_PTR, // EETypeElementType.Pointer
				(byte)CorElementType.ELEMENT_TYPE_FNPTR, // EETypeElementType.FunctionPointer
				default, // Pad the map to 32 elements to enable range check elimination
				default,
				default,
				default
			};

			return (CorElementType)map[(int)ElementType];
		}
	}

	internal EETypeElementType ElementType => value->ElementType;

	[Intrinsic]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static EETypePtr EETypePtrOf<T>() 
	{
		// Compilers are required to provide a low level implementation of this method.
		throw new NotImplementedException();
	}

	public struct InterfaceCollection 
	{
		private MethodTable* value;

		internal InterfaceCollection(MethodTable* value) => this.value = value;

		public int Count => value->NumInterfaces;

		public EETypePtr this[int index] => new EETypePtr(value->InterfaceMap[index]);
	}

	public struct GenericArgumentCollection 
	{
		private MethodTableList arguments;
		private uint argumentCount;

		internal GenericArgumentCollection(uint argumentCount, MethodTableList arguments) 
		{
			this.argumentCount = argumentCount;
			this.arguments = arguments;
		}

		public int Length => (int)argumentCount;

		public EETypePtr this[int index] => new EETypePtr(arguments[index]);
	}
}
