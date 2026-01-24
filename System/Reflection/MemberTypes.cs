namespace System.Reflection;

public enum MemberTypes 
{
	Constructor = 1 << 0,
	Event = 1 << 1,
	Field = 1 << 2,
	Method = 1 << 3,
	Property = 1 << 4,
	TypeInfo = 1 << 5,
	Custom = 1 << 6,
	NestedType = 1 << 7,
	All = Constructor | Event | Field | Method | Property | TypeInfo | Custom | NestedType,
}
