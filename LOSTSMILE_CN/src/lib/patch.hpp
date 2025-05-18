#pragma once
#include <detours.h>
#ifdef _DEBUG
#define DEBUG_ONLY(...) __VA_ARGS__
#else
#define DEBUG_ONLY(...)
#endif

namespace Patch 
{
	struct Hooker
	{
		template<auto Fun>
		inline static decltype(Fun) Call;

		inline static auto Begin() -> void
		{
			::DetourTransactionBegin();
		}

		template<auto Fun>
		inline static auto Add(decltype(Fun) target) -> void
		{
			::DetourAttach({ &(Hooker::Call<Fun> = {target}) }, Fun);
		}

		template<auto Fun>
		inline static auto Add(const void* target) -> void
		{
			Hooker::Add<Fun>(reinterpret_cast<decltype(Fun)>(target));
		}

		inline static auto Commit() -> void
		{
			::DetourUpdateThread(::GetCurrentThread());
			::DetourTransactionCommit();
		}
	};

	namespace Mem
	{

		static auto MemWrite(LPVOID Addr, LPVOID Buf, SIZE_T Size) -> bool 
		{
			DWORD  Protect{ NULL };
			SIZE_T Written{ NULL };
			if (::VirtualProtect(Addr, Size, PAGE_EXECUTE_READWRITE, &Protect))
			{
				static_cast<void>(WriteProcessMemory(INVALID_HANDLE_VALUE, Addr, Buf, Size, &Written));
				static_cast<void>(VirtualProtect(Addr, Size, Protect, &Protect));
				return { Size == Written };
			}
			return { false };
		}
		
		static auto JmpWrite(LPVOID OrgAddr, LPVOID TarAddr) -> bool 
		{
			BYTE jmpAsm[]
			{
				// mov rax, addr;
				0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				// jmp rax
				0xFF, 0xE0
			};
			*reinterpret_cast<LPVOID*>(jmpAsm + 2) = { TarAddr };
			return { MemWrite(OrgAddr, jmpAsm, sizeof(jmpAsm)) };
		}
       
    }
	
}