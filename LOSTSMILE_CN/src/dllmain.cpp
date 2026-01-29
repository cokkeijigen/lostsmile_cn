#include <iostream>
#include <filesystem>
#include <windows.h>
#include <patch.hpp>
#include <console.hpp>

namespace LOSTSMILE
{
    static DEBUG_ONLY(std::unique_ptr<console::helper_t> xcout{ nullptr });
    static HMODULE UnityPlayerDll{ ::LoadLibraryW(L"UnityPlayer.dll") };
    static std::string TargetPath{ "/" PROJECT_NAME "/" };

    static auto WINAPI SetWindowTextW(HWND hWnd, LPCWSTR lpString) -> BOOL
    {
        return Patch::Hooker::Call<LOSTSMILE::SetWindowTextW>
        (
            hWnd,
            {
                std::wstring_view{ lpString } == L"LOSTSMILE" ?
                L"【星美岛绿茶品鉴中心】 LOSTSMILE 简体中文版 v" PROJECT_VERSION
                L" ※仅供学习交流使用，禁止一切直播录播和商用行为※" : 
                lpString
            }
        );
    }

    static auto __fastcall UnityPlayer_PathJoin(uintptr_t path1, uintptr_t path2, uint8_t symbol, uintptr_t output) -> uintptr_t
    {
        auto raw { reinterpret_cast<uintptr_t>(LOSTSMILE::UnityPlayerDll) + 0x875B20 };
        auto call{ reinterpret_cast<decltype(LOSTSMILE::UnityPlayer_PathJoin)*>(raw) };
        return { call(path1, path2, symbol, output) };
    }

    static auto __fastcall UnityPlayer_PathJoin_Hook(uintptr_t output, uintptr_t path1, uintptr_t path2) -> uintptr_t
    {
        *reinterpret_cast<int64_t*>(output + 0x00) = 0x00;
        *reinterpret_cast<uint8_t*>(output + 0x08) = 0x00;
        *reinterpret_cast<int64_t*>(output + 0x18) = 0x00;
        *reinterpret_cast<int32_t*>(output + 0x20) = 0x45;

        const auto str1{ *reinterpret_cast<const char**>(path1) };
        const auto str2{ *reinterpret_cast<const char**>(path2) };
        
        // DEBUG_ONLY(xcout->write("PathJoin_Hook{ %s, %s }\n", str1, str2));

        if (str2 != nullptr)
        {
            const auto target{ std::string{ LOSTSMILE::TargetPath }.append(str2) };
            if (::GetFileAttributesA(target.c_str()) != INVALID_FILE_ATTRIBUTES)
            {
                const auto len1{ *reinterpret_cast<uint64_t*>(path1 + 0x18) };
                const auto len2{ *reinterpret_cast<uint64_t*>(path2 + 0x18) };

                // 替换字符串和长度
                *reinterpret_cast<uint64_t*>(path1 + 0x18)    = 0x0llu;
                *reinterpret_cast<uint64_t*>(path2 + 0x18)    = target.size();
                *reinterpret_cast<const char**>(path1 + 0x00) = nullptr;
                *reinterpret_cast<const char**>(path2 + 0x00) = target.data();
                
                // 调用原来的函数进行拼接
                LOSTSMILE::UnityPlayer_PathJoin(path1, path2, '/', output);

                // 还原字符串和长度
                *reinterpret_cast<uint64_t*>(path1 + 0x18)    = len1;
                *reinterpret_cast<uint64_t*>(path2 + 0x18)    = len2;
                *reinterpret_cast<const char**>(path1 + 0x00) = str1;
                *reinterpret_cast<const char**>(path2 + 0x00) = str2;

                return { output };
            }
        }
        
        LOSTSMILE::UnityPlayer_PathJoin(path1, path2, 0x2F, output);
        
        return { output };
    }

    static auto __fastcall UnityPlayer_StringCheck(uintptr_t str, size_t length, char value) -> size_t
    {
        auto raw { reinterpret_cast<uintptr_t>(LOSTSMILE::UnityPlayerDll) + 0x1B63E0 };
        auto call{ reinterpret_cast<decltype(LOSTSMILE::UnityPlayer_StringCheck)*>(raw) };
        return { call(str, length, value) };
    }

    static auto __fastcall UnityPlayer_StrCat_Hook(uintptr_t dest, const char* src, size_t length) -> void
    {

        if (length == 0x00 || src == nullptr)
        {
            return;
        }

        if (*reinterpret_cast<char**>(dest) != nullptr)
        {
            std::string_view str1{ *reinterpret_cast<char**>(dest) }, str2{ src };
            auto is_replacement_attempt_needed
            {
                str1.size() > 2 && str1[1] == ':' &&
                str2.size() > 2 && str2[1] != ':' &&
                str1.rfind("_Data") != std::string_view::npos
            };

            if (is_replacement_attempt_needed)
            {
                // 这里进行路径替换
                auto target{ std::string{ LOSTSMILE::TargetPath }.append(str2) };
                if (::GetFileAttributesA(target.c_str()) != INVALID_FILE_ATTRIBUTES)
                {
                    *reinterpret_cast<char**>(dest)[0] = 0x00;
                    LOSTSMILE::UnityPlayer_StringCheck(dest, target.size(), 1);
                    std::copy(target.begin(), target.end(), *reinterpret_cast<char**>(dest));
                    return;
                }
            }
        }

        auto dest_str{  reinterpret_cast<char*>  (dest + 0x08) };
        auto dest_len{ *reinterpret_cast<size_t*>(dest + 0x18) };

        LOSTSMILE::UnityPlayer_StringCheck(dest, dest_len + length, 1);
        
        if (auto org_dest_str = *reinterpret_cast<char**>(dest))
        {
            dest_str = org_dest_str;
        }
        if (src >= dest_str && src < dest_str + dest_len)
        {
            src += (dest_str - reinterpret_cast<char*>(dest + 8));
        }
        std::copy_backward(src, src + length, dest_str + dest_len + length);
    }

    static auto INIT_ALL_PATCH(void) -> void
    {
        if (LOSTSMILE::UnityPlayerDll == nullptr)
        {
            auto uniMod{ ::GetModuleHandleW(L"UnityPlayer.dll") };
            if (uniMod != nullptr)
            {
                LOSTSMILE::UnityPlayerDll = { uniMod };
            }
            else
            {
                return;
            }
        }

        Patch::Hooker::Begin();
        Patch::Hooker::Add<LOSTSMILE::SetWindowTextW>(::SetWindowTextW);

        auto UnityPlayer_StrCat{ reinterpret_cast<uintptr_t>(LOSTSMILE::UnityPlayerDll) + 0x1B6070 };
        Patch::Hooker::Add<LOSTSMILE::UnityPlayer_StrCat_Hook>(reinterpret_cast<void*>(UnityPlayer_StrCat));

        auto UnityPlayer_PathJoin{ reinterpret_cast<uintptr_t>(LOSTSMILE::UnityPlayerDll) + 0x8759D0 };
        Patch::Hooker::Add<LOSTSMILE::UnityPlayer_PathJoin_Hook>(reinterpret_cast<void*>(UnityPlayer_PathJoin));

        Patch::Hooker::Commit();

        auto workpath{ std::filesystem::current_path().string() };
        {
            std::replace(workpath.begin(), workpath.end(), '\\', '/');
            LOSTSMILE::TargetPath.insert(0, workpath);
        }

        DEBUG_ONLY
        ({
            xcout = std::make_unique<console::helper_t>(L"" PROJECT_NAME " v" PROJECT_VERSION);
            if (::LoadLibraryW(L"MonoEnableDebugger.dll"))
            {
                xcout->writeline("MonoEnableDebugger!\n");
            }
        })
    }

    extern "C"
    {
        __declspec(dllexport) auto hook(void) -> void {}

        __declspec(dllexport) auto _patch_by_iTsukezigen_(void) -> const char*
        {
            return { "https://github.com/cokkeijigen/lostsmile_cn" };
        }

        DEBUG_ONLY(__declspec(dllexport)
        auto _cdecl DebugMessage(const wchar_t* message) -> void
        {
            xcout->write(message);
        })
        
        auto APIENTRY DllMain(HMODULE, DWORD ul_reason_for_call, LPVOID) -> BOOL
        {
            if (DLL_PROCESS_ATTACH == ul_reason_for_call)
            {
                LOSTSMILE::INIT_ALL_PATCH();
            }
            return { TRUE };
        }
    }
}