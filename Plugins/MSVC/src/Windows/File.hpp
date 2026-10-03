#include "pch.h"

// TODO: pchを使うまでもない物量なので単純化の為に外すことを検討
// TODO: WindowsではNT系のカーネルAPIを使う事で文字列の終端がヌル文字でなくとも動作するように修正

struct OpenContext
{
public:
    int32_t Access;
    int32_t Share;
    int32_t Mode;
    int32_t Attribute;
};

extern "C"
{
    __declspec(dllexport) void* __cdecl FileOpenUTF16(
        const wchar_t* path
        , const OpenContext* context
    )
    {
        return CreateFileW(
            path
            , context->Access
            , context->Share
            , nullptr
            , context->Mode
            , context->Attribute
            , nullptr
        );
    }

    __declspec(dllexport) void* __cdecl FileOpenReadUTF16(
        const wchar_t* path
    )
    {
        return CreateFileW(
            path
            , GENERIC_READ
            , 0
            , nullptr
            , OPEN_EXISTING
            , FILE_ATTRIBUTE_NORMAL
            , nullptr
        );
    }

    __declspec(dllexport) void* __cdecl FileOpenWriteUTF16(
        const wchar_t* path
    )
    {
        return CreateFileW(
            path
            , GENERIC_WRITE
            , 0
            , nullptr
            , OPEN_ALWAYS
            , FILE_ATTRIBUTE_NORMAL
            , nullptr
        );
    }

    __declspec(dllexport) int64_t __cdecl FileSize(
        void* handle
    )
    {
        LARGE_INTEGER result;

        // NOTE: 基本的にハンドルが不正でない限りは大抵正常系なので戻り値判定しない
        GetFileSizeEx(handle, &result);

        return result.QuadPart;
    }

    __declspec(dllexport) void __cdecl FileClose(
        void* handle
    )
    {
        CloseHandle(handle);
    }

    __declspec(dllexport) uint32_t __cdecl FileRead(
        void* handle
        , void* buffer
        , uint32_t length
    )
    {
        DWORD read;

        if (ReadFile(handle, buffer, length, &read, nullptr))
        {
            return read;
        }

        return -1;
    }

    __declspec(dllexport) uint32_t __cdecl FileWrite(
        void* handle
        , const void* buffer
        , uint32_t length
    )
    {
        DWORD written;

        if (WriteFile(handle, buffer, length, &written, nullptr))
        {
            return written;
        }

        return -1;
    }

    __declspec(dllexport) bool __cdecl FileFlush(
        void* handle
    )
    {
        return FlushFileBuffers(handle) != 0;
    }
}
