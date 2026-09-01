using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace NotBura.Core
{
    public interface IUUID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UUID FromCharSpan(ReadOnlySpan<char> span)
        {
            var high =  ReadValue(span,  0 + 0,  8);
            high <<= 8 * 2;
            high |=     ReadValue(span,  8 + 1,  4);
            high <<= 8 * 2;
            high |=     ReadValue(span, 12 + 2,  4);

            var low =   ReadValue(span, 16 + 3,  4);
            low <<= 8 * 6;
            low |=      ReadValue(span, 20 + 4, 12);

            return new(high, low);

            static ulong ReadValue(ReadOnlySpan<char> span, int offset, int length)
            {
                // NOTE: write byte spanやポインタで実装したものはこれより遅かった
                // 本格的な最適化を行えばより良い実装があるだろうが可読性も考慮し現状とする

                var result = 0UL;

                for (int i = 0; i < length; ++i)
                {
                    result <<= 4;
                    var c = span[offset + i];
                    var v = c <= '9'
                        ? c - '0'
                        : (c & ~('a' - 'A')) - 'A' + 10;
                    result |= (uint)v;
                }

                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ToString(void* source)
        {
            var span = (stackalloc char[8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + 12]);

            SetValue(span,  0 + 0,  8, 0x0000_0000_FFFF_FFFF & (*(ulong*)source) >> 32);
            span[ 8 + 0] = '-';
            SetValue(span,  8 + 1,  4, 0x0000_0000_0000_FFFF & (*(ulong*)source) >> 16);
            span[12 + 1] = '-';
            SetValue(span, 12 + 2,  4, 0x0000_0000_0000_FFFF & *(ulong*)source);
            span[16 + 2] = '-';
            SetValue(span, 16 + 3,  4, 0x0000_0000_0000_FFFF & (*((ulong*)source + 1)) >> 48);
            span[20 + 3] = '-';
            SetValue(span, 20 + 4, 12, 0x0000_FFFF_FFFF_FFFF & *((ulong*)source + 1));

            return new(span);

            static void SetValue(Span<char> span, int offset, int length, ulong value)
            {
                // NOTE: tableを活用する方法は1.2倍から2倍近く遅い
                // write char spanと同階層でTableを確保するなど工夫しても遅い
                //var table = (stackalloc char[]
                //{
                //    '0', '1', '2', '3', '4', '5', '6', '7',
                //    '8', '9', 'a', 'b', 'c', 'd', 'e', 'f',
                //});

                for (int i = 0; i < length; ++i)
                {
                    var c = 0xF & (value >> (4 * (length - 1 - i)));
                    span[offset + i] = c < 10
                        ? (char)('0' + c)
                        : (char)('a' + (c - 10));
                }
            }
        }
    }

#if UNITY_EDITOR
    [DebuggerDisplay("{ToString()}")]
#endif
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct UUID
        : IUUID
        , IEquatable<UUID>
        , IComparable<UUID>
    {
        [FieldOffset(0)] [SerializeField] private ulong m_high;
        [FieldOffset(8)] [SerializeField] private ulong m_low;

        public UUID(ulong high, ulong low)
        {
            m_high = high;
            m_low = low;
        }

        public unsafe bool Equals(UUID other)
        {
            fixed (void* pointer = &this)
            {
                return UnsafeUtility.MemCmp(pointer, &other, 16) == 0;
            }
        }

        public unsafe int CompareTo(UUID other)
        {
            fixed (void* pointer = &this)
            {
                return UnsafeUtility.MemCmp(pointer, &other, 16);
            }
        }

        [Obsolete("Call boxing method.")]
#pragma warning disable CS0809
        public override bool Equals(object obj)
#pragma warning restore CS0809
        {
            return obj is UUID cast && Equals(cast);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_high, m_low);
        }

        public override unsafe string ToString()
        {
            fixed (void* pointer = &this)
            {
                return IUUID.ToString(pointer);
            }
        }

        public static UUID FromCharSpan(ReadOnlySpan<char> span)
        {
            return IUUID.FromCharSpan(span);
        }
    }
}
