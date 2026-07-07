using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using Unity.Collections;

namespace NotBura.Packages
{
    public sealed class DirectFileReader
            : IDisposable
    {
        public unsafe ref struct Handle
        {
            private SafeMemoryMappedViewHandle m_handle;
            private byte* m_pointer;

            public Handle(SafeMemoryMappedViewHandle handle)
            {
                m_handle = handle;
                m_pointer = null;
                m_handle.AcquirePointer(ref m_pointer);
            }

            public ReadOnlySpan<byte> GetSpan(long offset, int length)
            {
                return new(m_pointer + offset, length);
            }

            public void Dispose()
            {
                m_pointer = null;
                m_handle.ReleasePointer();
                m_handle = null;
            }
        }

        private MemoryMappedFile m_file;
        private MemoryMappedViewAccessor m_accessor;

        public DirectFileReader(string path)
        {
            m_file = MemoryMappedFile.CreateFromFile(
                path,
                FileMode.Open,
                null,
                0,
                MemoryMappedFileAccess.Read
            );

            m_accessor = m_file.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        }

        public Handle GetHandle()
        {
            return new(m_accessor.SafeMemoryMappedViewHandle);
        }

        public ReadOnlySpan<byte> GetSpan(long offset, int length)
        {
            using var _handle = GetHandle();
            return _handle.GetSpan(offset, length);
        }

        public NativeArray<char> GetNativeCharArray(Encoding encoding, long offset, int length, Allocator allocator)
        {
            using var _handle = GetHandle();

            var _source = _handle.GetSpan(offset, length);

            var _decoder = encoding.GetDecoder();
            var _charCount = _decoder.GetCharCount(_source, true);

            var _destinatnion = new NativeArray<char>(
                _charCount
                , allocator
                , NativeArrayOptions.UninitializedMemory
            );

            _decoder.GetChars(_source, _destinatnion.AsSpan(), true);

            return _destinatnion;
        }

        public void Dispose()
        {
            if (m_accessor != null)
            {
                m_accessor.Dispose();
                m_accessor = null;
            }

            if (m_file != null)
            {
                m_file.Dispose();
                m_file = null;
            }
        }
    }
}
