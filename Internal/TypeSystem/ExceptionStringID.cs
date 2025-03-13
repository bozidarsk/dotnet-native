namespace Internal.TypeSystem;

public enum ExceptionStringID 
{
	ClassLoadGeneral,
	ClassLoadExplicitGeneric,
	ClassLoadBadFormat,
	ClassLoadExplicitLayout,
	ClassLoadValueClassTooLarge,
	ClassLoadRankTooLarge,

	ClassLoadInlineArrayFieldCount,
	ClassLoadInlineArrayLength,
	ClassLoadInlineArrayExplicit,

	MissingMethod,

	MissingField,

	FileLoadErrorGeneric,

	InvalidProgramDefault,
	InvalidProgramSpecific,
	InvalidProgramVararg,
	InvalidProgramCallVirtFinalize,
	InvalidProgramCallAbstractMethod,
	InvalidProgramCallVirtStatic,
	InvalidProgramNonStaticMethod,
	InvalidProgramGenericMethod,
	InvalidProgramNonBlittableTypes,
	InvalidProgramMultipleCallConv,

	BadImageFormatGeneric,
	BadImageFormatSpecific,

	MarshalDirectiveGeneric,

	AmbiguousMatchUnsafeAccessor,
}
