using System;
using System.Runtime.CompilerServices;

namespace NotBura.Core
{
    // NOTE: DB用の最適化とメモリアロケーション回避のため使用するstring置き換え構造体
    public unsafe readonly ref struct DBString
    {
        private readonly char* m_pointer;
        private readonly int m_length;

        #region constructor finalizer

        public DBString(char* pointer, int size)
        {
            m_pointer = pointer;
            m_length = size;
        }

        #endregion constructor finalizer

        #region interface method

        // NOTE: ref structなためインターフェースで実装できない
        public bool Equals(string other)
        {
            return new ReadOnlySpan<char>(m_pointer, m_length) == other.AsSpan();
        }

        public bool Equals(DBString other)
        {
            return m_pointer == other.m_pointer
                && m_length == other.m_length;
        }

        #endregion interface method

        #region override method

        public override string ToString()
        {
            return new(m_pointer, 0, m_length);
        }

        #endregion override method

        #region public method

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> AsSpan()
        {
            return new ReadOnlySpan<char>(m_pointer, m_length);
        }

        #endregion public method
    }
}
