using System;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public struct NotFileReader
        : IDisposable
    {
        private unsafe void* m_handle;

        public unsafe bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_handle is not null;
        }

        public unsafe bool IsInvalid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_handle is null;
        }

        internal unsafe NotFileReader(void* handle)
        {
            m_handle = handle;
        }

        public unsafe void Dispose()
        {
            if (m_handle is null)
            {
                return;
            }

            Plugin.FileClose(m_handle);
            m_handle = null!;
        }

        public unsafe long GetSize()
        {
            return Plugin.FileSize(m_handle);
        }

        public unsafe uint Read(byte[] destination)
        {
            fixed (byte* _pointer = destination)
            {
                return Plugin.FileRead(m_handle, _pointer, unchecked((uint)destination.Length));
            }
        }

        public unsafe uint Read(Span<byte> destination)
        {
            fixed (byte* _pointer = destination)
            {
                return Plugin.FileRead(m_handle, _pointer, unchecked((uint)destination.Length));
            }
        }

        public unsafe uint Read(void* destination, uint length)
        {
            return Plugin.FileRead(m_handle, destination, length);
        }
    }
}
