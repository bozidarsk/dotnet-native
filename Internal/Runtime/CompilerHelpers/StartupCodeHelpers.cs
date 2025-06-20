using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Internal.Runtime;

namespace Internal.Runtime.CompilerHelpers;

internal static unsafe class StartupCodeHelpers 
{
	[UnmanagedCallersOnly(EntryPoint = "InitializeModules")]
	private static void InitializeModules(nint osModule, nint* pModuleHeaders, int count, nint* pClasslibFunctions, int nClasslibFunctions) 
	{
		while (*pModuleHeaders != 0) 
		{
			TypeManager typeManager = new TypeManager(osModule, *pModuleHeaders, pClasslibFunctions, (uint)nClasslibFunctions);

			RunInitializer(typeManager, ReadyToRunSectionType.EagerCctor);

			pModuleHeaders++;
		}
	}

	private static void RunInitializer(TypeManager typeManager, ReadyToRunSectionType section) 
	{
		byte* pInitializers = (byte*)typeManager.GetModuleSection(section, out int length);

		for (
			byte* pCurrent = pInitializers;
			pCurrent < (pInitializers + length);
			pCurrent += MethodTable.SupportsRelativePointers ? sizeof(int) : sizeof(nint))
		{
			var initializer = MethodTable.SupportsRelativePointers ? (delegate*<void>)ReadRelPtr32(pCurrent) : *(delegate*<void>*)pCurrent;
			initializer();
		}

		static void* ReadRelPtr32(void* address) => (byte*)address + *(int*)address;
	}
}
