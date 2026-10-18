#include "pch.h"

#pragma comment(lib, "ws2_32.lib")

extern "C"
{
    __declspec(dllexport) int32_t WSAOpen(
        void* result
    )
    {
        return ::WSAStartup(MAKEWORD(2, 2), static_cast<WSADATA*>(result));
    }

    __declspec(dllexport) void WSAClose()
    {
        ::WSACleanup();
    }

    __declspec(dllexport) void* __cdecl SocketOpenUDP()
    {
        SOCKET _socket = ::socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
        return reinterpret_cast<void*>(_socket);
    }

    __declspec(dllexport) int32_t __cdecl SocketClose(
        void* socket
    )
    {
        return ::closesocket(reinterpret_cast<SOCKET>(socket));
    }

    __declspec(dllexport) int32_t __cdecl SocketAddress(
        const char* address
        , uint16_t port
        , void* destination
    )
    {
        sockaddr_in _buffer{};
        _buffer.sin_family = AF_INET;
        _buffer.sin_port = ::htons(port);

        int32_t _result = ::inet_pton(AF_INET, address, &_buffer);

        *reinterpret_cast<sockaddr_in*>(destination) = _buffer;

        return _result;
    }

    __declspec(dllexport) int32_t __cdecl SocketSendTo(
        void* socket
        , void* buffer
        , int32_t length
        , void* address
    )
    {
        return ::sendto(
            reinterpret_cast<SOCKET>(socket)
            , reinterpret_cast<const char*>(buffer)
            , length
            , 0
            , reinterpret_cast<sockaddr*>(address)
            , sizeof(sockaddr)
        );
    }
}
