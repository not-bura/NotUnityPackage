using System;
using Unity.Collections;

namespace NotBura.Core
{
    // NOTE: 静的動的で実装を分けて最適化するか考える
    public sealed class NotTable<T>
        : IDisposable
        where T : unmanaged
    {
        public delegate bool WherePredicate(NotHandle<T> handle);

        private NativeArray<byte> m_binary;

        #region constructor finalizer

        public unsafe NotTable(int capacity = 256)
        {
            var size = sizeof(T) * capacity;
            m_binary = new NativeArray<byte>(
                size
                , Allocator.Persistent
                , NativeArrayOptions.UninitializedMemory
            );
        }

        public NotTable(ReadOnlySpan<byte> binary)
        {
            m_binary = new NativeArray<byte>(
                binary.Length
                , Allocator.Persistent
                , NativeArrayOptions.UninitializedMemory
            );
            binary.CopyTo(m_binary);
        }

        public unsafe NotTable(ReadOnlySpan<T> values)
        {
            var size = sizeof(T) * values.Length;
            m_binary = new NativeArray<byte>(
                size
                , Allocator.Persistent
                , NativeArrayOptions.UninitializedMemory
            );

            fixed (byte* ptr = m_binary.AsSpan())
            {
                var span = new Span<T>(ptr, values.Length);
                values.CopyTo(span);
            }
        }

        #endregion constructor finalizer

        public unsafe int Insert(in T value)
        {
            int size = sizeof(T);

            return 0;
        }

        public int Where(WherePredicate predicate)
        {
            return 0;
        }

        public int Update(in T value)
        {
            return 0;
        }

        public int Delete(in T value)
        {
            return 0;
        }

        public void Dispose()
        {
            if (m_binary.IsCreated)
            {
                m_binary.Dispose();
            }
        }
    }
}
