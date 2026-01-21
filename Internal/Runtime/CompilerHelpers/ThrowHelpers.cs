using System;
using System.IO;
using System.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using Internal.TypeSystem;

namespace Internal.Runtime.CompilerHelpers;

internal static class ThrowHelpers 
{
	[RuntimeExport("throw_BodyRemoved")]
	public static void ThrowBodyRemoved() => throw new NotSupportedException();

	[RuntimeExport("throw_FeatureBodyRemoved")]
	public static void ThrowFeatureBodyRemoved() => throw new NotSupportedException();

	[RuntimeExport("throw_InstanceBodyRemoved")]
	public static void ThrowInstanceBodyRemoved() => throw new NotSupportedException();

	[RuntimeExport("throw_UnavailableType")]
	public static void ThrowUnavailableType() => throw new TypeLoadException();

	[RuntimeExport("throw_OverflowException")]
	public static void ThrowOverflowException() => throw new OverflowException();

	[RuntimeExport("throw_IndexOutOfRangeException")]
	public static void ThrowIndexOutOfRangeException() => throw new IndexOutOfRangeException();

	[RuntimeExport("throw_NullReferenceException")]
	public static void ThrowNullReferenceException() => throw new NullReferenceException();

	[RuntimeExport("throw_DivideByZeroException")]
	public static void ThrowDivideByZeroException() => throw new DivideByZeroException();

	[RuntimeExport("throw_ArrayTypeMismatchException")]
	public static void ThrowArrayTypeMismatchException() => throw new ArrayTypeMismatchException();

	[RuntimeExport("throw_PlatformNotSupportedException")]
	public static void ThrowPlatformNotSupportedException() => throw new PlatformNotSupportedException();

	[RuntimeExport("throw_NotImplementedException")]
	public static void ThrowNotImplementedException() => throw new NotImplementedException();

	[RuntimeExport("throw_NotSupportedException")]
	public static void ThrowNotSupportedException() => throw new NotSupportedException();

	[RuntimeExport("throw_AccessViolationException")]
	public static void ThrowAccessViolationException() => throw new AccessViolationException();

	[RuntimeExport("throw_BadImageFormatException")]
	public static void ThrowBadImageFormatException(ExceptionStringID id) => throw new BadImageFormatException();

	[RuntimeExport("throw_TypeLoadException")]
	public static void ThrowTypeLoadException(ExceptionStringID id, string className, string typeName) => throw new TypeLoadException($"{className} {typeName}");

	[RuntimeExport("throw_TypeLoadExceptionWithArgument")]
	public static void ThrowTypeLoadExceptionWithArgument(ExceptionStringID id, string className, string typeName, string messageArg) => throw new TypeLoadException(messageArg);

	[RuntimeExport("throw_MissingMethodException")]
	public static void ThrowMissingMethodException(ExceptionStringID id, string methodName) => throw new MissingMethodException(methodName);

	[RuntimeExport("throw_MissingFieldException")]
	public static void ThrowMissingFieldException(ExceptionStringID id, string fieldName) => throw new MissingFieldException(fieldName);

	[RuntimeExport("throw_FileNotFoundException")]
	public static void ThrowFileNotFoundException(ExceptionStringID id, string fileName) => throw new FileNotFoundException(fileName);

	[RuntimeExport("throw_InvalidProgramException")]
	public static void ThrowInvalidProgramException(ExceptionStringID id) => throw new InvalidProgramException();

	[RuntimeExport("throw_InvalidProgramExceptionWithArgument")]
	public static void ThrowInvalidProgramExceptionWithArgument(ExceptionStringID id, string methodName) => throw new InvalidProgramException(methodName);

	[RuntimeExport("throw_AmbiguousMatchException")]
	public static void ThrowAmbiguousMatchException(ExceptionStringID id) => throw new AmbiguousMatchException();

	[RuntimeExport("throw_ArgumentException")]
	public static void ThrowArgumentException() => throw new ArgumentException();

	[RuntimeExport("throw_ArgumentOutOfRangeException")]
	public static void ThrowArgumentOutOfRangeException() => throw new ArgumentOutOfRangeException();
}
