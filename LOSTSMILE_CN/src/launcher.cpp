#include <Windows.h>
#include <detours.h>
#pragma comment(linker, "/subsystem:\"windows\" /entry:\"mainCRTStartup\"")

auto main(void) -> int 
{
	STARTUPINFOW si{};
	PROCESS_INFORMATION pi{};
	auto created_successfully
	{
		::DetourCreateProcessWithDllW
		(
			{ L"LOSTSMILE.exe" },
			{ NULL },
			{ NULL }, 
			{ NULL }, 
			{ FALSE },
			{ CREATE_SUSPENDED },
			{ NULL }, { NULL },
			{ &si },
			{ &pi },
			{ ".\\LOSTSMILE_CN\\LOSTSMILE_CN.dll" },
			{ NULL }
		)
	};
	if (created_successfully)
	{
		::ResumeThread(pi.hThread);
		::CloseHandle(pi.hThread);
		::CloseHandle(pi.hProcess);
	}

	return {};
}