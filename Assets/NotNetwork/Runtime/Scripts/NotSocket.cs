using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NotBura.Packages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NotAddress
    {
        private const int SUCCESS = 1;

        public ushort   Family;
        public ushort   Port;
        public uint     Address;
        public ulong    Zero;

        public static unsafe bool TryCreate(string address, ushort port, ref NotAddress result)
        {
            fixed (char* _pointer = address)
            fixed (void* _destination = &result)
            {
                var _result = Plugin.SocketAddress(_pointer, port, _destination);

                if (_result is SUCCESS)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public struct NotWSAHandle
        : IDisposable
    {
        private int _state;

        public int State
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _state;
        }

        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _state is 0;
        }

        public bool IsInvalid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _state is not 0;
        }

        private NotWSAHandle(int state)
        {
            _state = state;
        }

        public void Dispose()
        {
            if (_state is 0)
            {
                return;
            }

            Plugin.WSAClose();
        }

        public static unsafe NotWSAHandle Create(void* context)
        {
            var _result = Plugin.WSAOpen(context);
            return new(_result);
        }
    }

    public struct NotSocket
        : IDisposable
    {
        private const nint INVALID  = ~0;
        private const nint ERROR    = -1;

        private unsafe void* m_handle;

        private unsafe NotSocket(void* handle)
        {
            m_handle = handle;
        }

        public unsafe void Dispose()
        {
            if (m_handle is null)
            {
                return;
            }

            Plugin.SocketClose(m_handle);
            m_handle = null!;
        }

        public static unsafe bool TryCreate(out NotSocket result)
        {
            var _handle = Plugin.SocketOpenUDP();

            if ((nint)_handle is INVALID or ERROR)
            {
                result = default;
                return false;
            }

            result = new(_handle);
            return true;
        }

        public unsafe int Send(void* buffer, int length, in NotAddress address)
        {
            fixed (void* _address = &address)
            {
                return Plugin.SocketSendTo(m_handle, buffer, length, _address);
            }
        }

        public unsafe int Send(ReadOnlySpan<byte> buffer, in NotAddress address)
        {
            fixed (void* _buffer = buffer)
            fixed (void* _address = &address)
            {
                return Plugin.SocketSendTo(m_handle, _address, buffer.Length, _address);
            }
        }
    }
}
