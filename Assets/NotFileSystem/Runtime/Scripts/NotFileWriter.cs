using System;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public struct NotFileWriter
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

        internal unsafe NotFileWriter(void* handle)
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

        public unsafe uint Write(string source)
        {
            fixed (char* _pointer = source)
            {
                return Plugin.FileWrite(m_handle, _pointer, unchecked((uint)source.Length << 1));
            }
        }

        public unsafe uint Write(ReadOnlySpan<char> source)
        {
            fixed (char* _pointer = source)
            {
                return Plugin.FileWrite(m_handle, _pointer, unchecked((uint)source.Length << 1));
            }
        }

        public unsafe uint Write(char* source, uint length)
        {
            return Plugin.FileWrite(m_handle, source, length);
        }

        public unsafe uint Write(ReadOnlySpan<byte> source)
        {
            fixed (byte* _pointer = source)
            {
                return Plugin.FileWrite(m_handle, _pointer, unchecked((uint)source.Length));
            }
        }

        public unsafe uint Write(byte* source, uint length)
        {
            return Plugin.FileWrite(m_handle, source, length);
        }

        public unsafe bool Flush()
        {
            return Plugin.FileFlush(m_handle);
        }
    }
}
