#define _CRT_SECURE_NO_WARNINGS
#define WIN32_LEAN_AND_MEAN
#include <iostream>
#include <filesystem>
#include <windows.h>

namespace Hook::Mem {

    bool MemWrite(intptr_t Addr, int8_t* Buf, size_t Size) {
        DWORD oldPro;
        SIZE_T wirteBytes = 0;
        if (VirtualProtect((VOID*)Addr, Size, PAGE_EXECUTE_READWRITE, &oldPro)) {
            WriteProcessMemory(INVALID_HANDLE_VALUE, (VOID*)Addr, Buf, Size, &wirteBytes);
            VirtualProtect((VOID*)Addr, Size, oldPro, &oldPro);
            return Size == wirteBytes;
        }
        return false;
    }

    bool JmpWrite(intptr_t orgAddr, intptr_t tarAddr) {
        int8_t jmp_write[5] = { 0xe9, 0x0, 0x0, 0x0, 0x0 };
        tarAddr = tarAddr - orgAddr - 5;
        memcpy(jmp_write + 1, &tarAddr, 4);
        return MemWrite(orgAddr, jmp_write, 5);
    }
}

namespace Hook::type {
    typedef intptr_t(__fastcall* sub_180875B20ptr)(intptr_t ipStr1, intptr_t ipStr2, char symbol, intptr_t opStr);
}

namespace Hook {

    intptr_t UnityPlayerBaseAddr;
    std::string cn_path("/LOSTSMILE_CN/");
    type::sub_180875B20ptr sub_180875B20;

    void init() {
        UnityPlayerBaseAddr = (intptr_t)GetModuleHandle(L"unityplayer.dll");
        sub_180875B20 = (type::sub_180875B20ptr)(UnityPlayerBaseAddr + 0x875B20);
        std::filesystem::path cur_path = std::filesystem::current_path();
        std::string g_path = cur_path.string();
        std::replace(g_path.begin(), g_path.end(), '\\', '/');
        cn_path.insert(0, g_path);
    }

    intptr_t __fastcall sub_1808759D0(intptr_t opStr, intptr_t ipStr1, intptr_t ipStr2) {

        *((int64_t*)(opStr + 0x00)) = 0x00;
        *((int64_t*)(opStr + 0x18)) = 0x00;
        *((int32_t*)(opStr + 0x20)) = 0x45;
        *((int8_t *)(opStr + 0x08)) = 0x00;
        char* src_str1 = (char*)*(intptr_t*)ipStr1;
        char* src_str2 = (char*)*(intptr_t*)ipStr2;

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

    void start() {
        Mem::JmpWrite(UnityPlayerBaseAddr + 0x8759D0, (intptr_t)sub_1808759D0);
    }
}

extern "C" __declspec(dllexport) void hook(void) {}
BOOL APIENTRY DllMain( HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        Hook::init();
        Hook::start();
    #ifdef _DEBUG
        //AllocConsole();
        //freopen("CONOUT$", "w", stdout);
        //freopen("CONIN$", "r", stdin);
        //system("pause");
    #endif 
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

