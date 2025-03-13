using System;
using System.IO;
using System.Runtime;
using System.Reflection;
using Internal.TypeSystem;

namespace Internal.Runtime.CompilerHelpers;

public static class ThrowHelpers 
{
	internal static void ThrowBodyRemoved() => throw new NotSupportedException();
	internal static void ThrowFeatureBodyRemoved() => throw new NotSupportedException();
	internal static void ThrowInstanceBodyRemoved() => throw new NotSupportedException();
	internal static void ThrowUnavailableType() => throw new TypeLoadException();
	public static void ThrowOverflowException() => throw new OverflowException();
	public static void ThrowIndexOutOfRangeException() => throw new IndexOutOfRangeException();
	public static void ThrowNullReferenceException() => throw new NullReferenceException();
	public static void ThrowDivideByZeroException() => throw new DivideByZeroException();
	public static void ThrowArrayTypeMismatchException() => throw new ArrayTypeMismatchException();
	public static void ThrowPlatformNotSupportedException() => throw new PlatformNotSupportedException();
	public static void ThrowNotImplementedException() => throw new NotImplementedException();
	public static void ThrowNotSupportedException() => throw new NotSupportedException();
	public static void ThrowBadImageFormatException(ExceptionStringID id) => throw new BadImageFormatException();
	public static void ThrowTypeLoadException(ExceptionStringID id, string className, string typeName) => throw new TypeLoadException($"{className} {typeName}");
	public static void ThrowTypeLoadExceptionWithArgument(ExceptionStringID id, string className, string typeName, string messageArg) => throw new TypeLoadException(messageArg);
	public static void ThrowMissingMethodException(ExceptionStringID id, string methodName) => throw new MissingMethodException(methodName);
	public static void ThrowMissingFieldException(ExceptionStringID id, string fieldName) => throw new MissingFieldException(fieldName);
	public static void ThrowFileNotFoundException(ExceptionStringID id, string fileName) => throw new FileNotFoundException(fileName);
	public static void ThrowInvalidProgramException(ExceptionStringID id) => throw new InvalidProgramException();
	public static void ThrowInvalidProgramExceptionWithArgument(ExceptionStringID id, string methodName) => throw new InvalidProgramException(methodName);
	public static void ThrowAmbiguousMatchException(ExceptionStringID id) => throw new AmbiguousMatchException();
	public static void ThrowArgumentException() => throw new ArgumentException();
	public static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException();
}
