namespace System.Runtime.InteropServices;

public enum UnmanagedType 
{
	Bool = 0x2,
	I1 = 0x3,
	U1 = 0x4,
	I2 = 0x5,
	U2 = 0x6,
	I4 = 0x7,
	U4 = 0x8,
	I8 = 0x9,
	U8 = 0xa,
	R4 = 0xb,
	R8 = 0xc,
	[Obsolete("Marshalling as Currency may be unavailable in future releases.")]
	Currency = 0xf,
	BStr = 0x13,
	LPStr = 0x14,
	LPWStr = 0x15,
	LPTStr = 0x16,
	ByValTStr = 0x17,
	IUnknown = 0x19,
	IDispatch = 0x1a,
	Struct = 0x1b,
	Interface = 0x1c,
	SafeArray = 0x1d,
	ByValArray = 0x1e,
	SysInt = 0x1f,
	SysUInt = 0x20,
	[Obsolete("Marshalling as VBByRefString may be unavailable in future releases.")]
	VBByRefStr = 0x22,
	[Obsolete("Marshalling as AnsiBStr may be unavailable in future releases.")]
	AnsiBStr = 0x23,
	[Obsolete("Marshalling as TBstr may be unavailable in future releases.")]
	TBStr = 0x24,
	VariantBool = 0x25,
	FunctionPtr = 0x26,
	[Obsolete("Marshalling arbitrary types may be unavailable in future releases. Specify the type you wish to marshal as.")]
	AsAny = 0x28,
	LPArray = 0x2a,
	LPStruct = 0x2b,
	CustomMarshaler = 0x2c,
	Error = 0x2d,
	IInspectable = 0x2e,
	HString = 0x2f,
	LPUTF8Str = 0x30,
}
