#define _CRT_SECURE_NO_WARNINGS
#define WIN32_LEAN_AND_MEAN
#include <iostream>
#include <filesystem>
#include <windows.h>

namespace Hook::Mem {

    bool MemWrite(uintptr_t Addr, uint8_t* Buf, size_t Size) {
        DWORD oldPro;
        SIZE_T wirteBytes = 0;
        if (VirtualProtect((VOID*)Addr, Size, PAGE_EXECUTE_READWRITE, &oldPro)) {
            WriteProcessMemory(GetCurrentProcess(), (VOID*)Addr, Buf, Size, &wirteBytes);
            VirtualProtect((VOID*)Addr, Size, oldPro, &oldPro);
            printf("MemWrite %p %zx bytes\n", (void*)Addr, wirteBytes);
            return Size == wirteBytes;
        }
        return false;
    }

    bool JmpWrite(uintptr_t orgAddr, uintptr_t tarAddr) {
        printf("JmpWrite orgin=%p, tagetr=%p\n", (void*)orgAddr, (void*)tarAddr);
        uint8_t jmp_write[] = { 0x48, 0xb8,  // mov rax, addr; jmp rax
                0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,  
                0xff, 0xe0};
        memcpy(jmp_write + 2, &tarAddr, 8);
        return MemWrite(orgAddr, jmp_write, sizeof(jmp_write));
    }
}

namespace Hook::type {
    typedef intptr_t(__fastcall* sub_180875B20ptr)(intptr_t ipStr1, intptr_t ipStr2, char symbol, intptr_t opStr);
}

namespace Hook {

    uintptr_t UnityPlayerBaseAddr;
    std::string cn_path("/LOSTSMILE_CN/");
    type::sub_180875B20ptr sub_180875B20;

    intptr_t __fastcall sub_1808759D0(intptr_t opStr, intptr_t ipStr1, intptr_t ipStr2) {

        *((int64_t*)(opStr + 0x00)) = 0x00;
        *((int64_t*)(opStr + 0x18)) = 0x00;
        *((int32_t*)(opStr + 0x20)) = 0x45;
        *((int8_t *)(opStr + 0x08)) = 0x00;
        char* src_str1 = (char*)*(intptr_t*)ipStr1;
        char* src_str2 = (char*)*(intptr_t*)ipStr2;
        printf("in sub_1808759D0 ipstr1=%s, ipstr2=%s\n", src_str1, src_str2);

        if (src_str2 && GetFileAttributesA(std::string(cn_path + src_str2).c_str()) != -1) {
            size_t length = *(intptr_t*)(ipStr1 + 0x18);
            // 重定向资源路径
            *(intptr_t*)(ipStr1 + 0x00) = (intptr_t)cn_path.c_str();
            // 替换字符串长度
            *(intptr_t*)(ipStr1 + 0x18) = cn_path.size();
            // 调用原来的函数进行拼接
            sub_180875B20(ipStr1, ipStr2, 0x2F, opStr);
            // 需要还原，不然会出现问题
            *(intptr_t*)(ipStr1 + 0x00) = (intptr_t)src_str1;
            *(intptr_t*)(ipStr1 + 0x18) = length;
        }
        else {
            sub_180875B20(ipStr1, ipStr2, 0x2F, opStr);
        }
        return opStr;
    }

    void init() {
        HMODULE hmod = LoadLibraryA("UnityPlayer.dll");
        UnityPlayerBaseAddr = (uintptr_t)GetModuleHandle(L"unityplayer.dll");
        printf("UnityPlayer hmod=%p, UnityPlayerBaseAddr %p\n", hmod, (void*)UnityPlayerBaseAddr);

        sub_180875B20 = (type::sub_180875B20ptr)(UnityPlayerBaseAddr + 0x875B20);
        printf("sub_180875B20 %p, rva=%zx, sub_1808759D0=%p\n",
            (void*)sub_180875B20, (size_t)sub_180875B20 - UnityPlayerBaseAddr, sub_1808759D0);
        std::filesystem::path cur_path = std::filesystem::current_path();
        std::string g_path = cur_path.string();
        std::replace(g_path.begin(), g_path.end(), '\\', '/');
        cn_path.insert(0, g_path);
    }

    void start() {
        Mem::JmpWrite(UnityPlayerBaseAddr + 0x8759D0, (intptr_t)sub_1808759D0);
    }
}

extern "C" __declspec(dllexport) void hook(void) {}
BOOL APIENTRY DllMain( HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
#ifdef _DEBUG
        AllocConsole();
        freopen("CONOUT$", "w", stdout);
        freopen("CONIN$", "r", stdin);
        system("pause");
#endif 
        Hook::init();
        Hook::start();

        break;
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
        break;
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

