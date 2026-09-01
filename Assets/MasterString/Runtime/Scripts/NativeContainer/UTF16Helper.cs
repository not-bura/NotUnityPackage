using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    public static class UTF16Helper
    {
        public static unsafe uint GetByteCount(string[] source)
        {
            if (source is null || source.Length == 0)
            {
                return 0;
            }

            var result = 0U;

            for (int i = 0; i < source.Length; ++i)
            {
                result += (uint)source[i].Length;
            }

            return result << 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint GetByteCount(string source)
        {
            return (uint)source.Length << 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint GetByteCount(ReadOnlySpan<char> source)
        {
            return (uint)source.Length << 1;
        }
    }
}
