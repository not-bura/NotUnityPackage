#include "pch.h"
#include <windows.h>
#include <cstdint>

extern "C" __declspec(dllexport)
void* FileOpenUTF16(const wchar_t* path)
{
    return CreateFileW(
        path
        , GENERIC_READ | GENERIC_WRITE
        , FILE_SHARE_READ | FILE_SHARE_WRITE
        , nullptr
        , OPEN_ALWAYS
        , FILE_ATTRIBUTE_NORMAL
        , nullptr
    );
}

extern "C" __declspec(dllexport)
void* FileOpenReadUTF16(const wchar_t* path)
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

extern "C" __declspec(dllexport)
void* FileOpenWriteUTF16(const wchar_t* path)
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

extern "C" __declspec(dllexport)
int64_t FileSize(void* handle)
{
    LARGE_INTEGER result;

    GetFileSizeEx(handle, &result);

    return result.QuadPart;
}

extern "C" __declspec(dllexport)
void* FileOpenUTF8(const uint8_t* path, int length)
{
    // ここでUTF-8 → UTF-16
    int wlen = MultiByteToWideChar(
        CP_UTF8
        , MB_ERR_INVALID_CHARS
        , reinterpret_cast<const char*>(path)
        , length
        , nullptr
        , 0
    );

    if (wlen <= 0)
    {
        return nullptr;
    }

    wchar_t* wpath = new wchar_t[wlen + 1];

    MultiByteToWideChar(
        CP_UTF8
        , MB_ERR_INVALID_CHARS
        , reinterpret_cast<const char*>(path)
        , length
        , wpath
        , wlen
    );

    wpath[wlen] = L'\0';

    HANDLE h = CreateFileW(
        wpath
        , GENERIC_READ | GENERIC_WRITE
        , FILE_SHARE_READ | FILE_SHARE_WRITE
        , nullptr
        , OPEN_ALWAYS
        , FILE_ATTRIBUTE_NORMAL
        , nullptr
    );

    delete[] wpath;

    if (h == INVALID_HANDLE_VALUE)
    {
        return nullptr;
    }

    return h;
}

extern "C" __declspec(dllexport)
void FileClose(void* handle)
{
    if (handle == nullptr)
    {
        return;
    }

    CloseHandle(static_cast<HANDLE>(handle));
}

extern "C" __declspec(dllexport)
uint32_t FileRead(
    void* handle
    , void* buffer
    , uint32_t length
)
{
    DWORD read;

    if (ReadFile(static_cast<HANDLE>(handle), buffer, length, &read, nullptr))
    {
        return read;
    }

    return -1;
}

extern "C" __declspec(dllexport)
uint32_t FileWrite(
    void* handle
    , const void* buffer
    , uint32_t length
)
{
    DWORD written;

    if (WriteFile(static_cast<HANDLE>(handle), buffer, length, &written, nullptr))
    {
        return written;
    }

    return -1;
}

extern "C" __declspec(dllexport)
bool FileFlush(
    void* handle
)
{
    return FlushFileBuffers(static_cast<HANDLE>(handle)) != 0;
}
