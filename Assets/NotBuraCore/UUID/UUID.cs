using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NotBura.Core
{
    public interface IUUID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(ulong fromHigh, ulong fromLow, ulong toHigh, ulong toLow)
        {
            return fromHigh == toHigh
                && fromLow == toLow;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTo(ulong fromHigh, ulong fromLow, ulong toHigh, ulong toLow)
        {
            if (fromHigh > toHigh)
            {
                return 1;
            }

            if (fromHigh < toHigh)
            {
                return -1;
            }

            if (fromLow > toLow)
            {
                return 1;
            }

            if (fromLow < toLow)
            {
                return -1;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (ulong high, ulong low) FromCharSpan(ReadOnlySpan<char> span)
        {
            var high =  ReadValue(span,  0 + 0,  8);
            high <<= 4 * 4;
            high |=     ReadValue(span,  8 + 1,  4);
            high <<= 4 * 4;
            high |=     ReadValue(span, 12 + 2,  4);

            var low =   ReadValue(span, 16 + 3,  4);
            low <<= 12 * 4;
            low |=      ReadValue(span, 20 + 4, 12);

            return (high, low);

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
        public static int GetHashCode(ulong high, ulong low)
        {
            return HashCode.Combine(high, low);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(ulong high, ulong low)
        {
            var span = (stackalloc char[8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + 12]);

            SetValue(span,  0 + 0,  8, 0x0000_0000_FFFF_FFFF & (high >> 8 * 4));
            span[8 + 0] = '-';
            SetValue(span,  8 + 1,  4, 0x0000_0000_0000_FFFF & (high >> 4 * 4));
            span[12 + 1] = '-';
            SetValue(span, 12 + 2,  4, 0x0000_0000_0000_FFFF & (high));
            span[16 + 2] = '-';
            SetValue(span, 16 + 3,  4, 0x0000_0000_0000_FFFF & (low >> 12 * 4));
            span[20 + 3] = '-';
            SetValue(span, 20 + 4, 12, 0x0000_FFFF_FFFF_FFFF & (low));

            return new string(span);

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

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct UUID
        : IUUID
        , IEquatable<UUID>
        , IComparable<UUID>
    {
        [SerializeField] private ulong m_high;
        [SerializeField] private ulong m_low;

        public UUID(ulong high, ulong low)
        {
            m_high = high;
            m_low = low;
        }

        public readonly bool Equals(UUID other)
        {
            return IUUID.Equals(m_high, m_low, other.m_high, other.m_low);
        }

        public readonly int CompareTo(UUID other)
        {
            return IUUID.CompareTo(m_high, m_low, other.m_high, other.m_low);
        }

        [Obsolete("Call boxing method.")]
#pragma warning disable CS0809
        public override readonly bool Equals(object obj)
#pragma warning restore CS0809
        {
            return obj is UUID cast && Equals(cast);
        }

        public override readonly int GetHashCode()
        {
            return IUUID.GetHashCode(m_high, m_low);
        }

        public override readonly string ToString()
        {
            return IUUID.ToString(m_high, m_low);
        }

        public static UUID FromCharSpan(ReadOnlySpan<char> span)
        {
            var result = IUUID.FromCharSpan(span);
            return new(result.high, result.low);
        }
    }
}
