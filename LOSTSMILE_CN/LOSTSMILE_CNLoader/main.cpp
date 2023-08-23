#include <Windows.h>
#include "../detours.h"
#pragma comment(lib, "..\\detours.lib")
#pragma comment(linker, "/subsystem:\"windows\" /entry:\"mainCRTStartup\"")

int main() {
	STARTUPINFOW si;
	PROCESS_INFORMATION pi;
	ZeroMemory(&si, sizeof(si));
	ZeroMemory(&pi, sizeof(pi));
	if (DetourCreateProcessWithDllW(L"LOSTSMILE.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED,
		NULL, NULL, &si, &pi, (LPCSTR)".\\LOSTSMILE_CN\\LOSTSMILE_CN.dll", NULL)) {
		ResumeThread(pi.hThread);
		CloseHandle(pi.hThread);
		CloseHandle(pi.hProcess);
	}
}