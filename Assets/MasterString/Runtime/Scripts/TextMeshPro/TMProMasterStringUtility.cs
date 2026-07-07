using System;

namespace NotBura.Packages
{
    public static class TMProMasterStringUtility
    {
        public static bool IsValidUTF16(ReadOnlySpan<char> source, int read)
        {
            return IsHexDigit(source[read + 0])
                && IsHexDigit(source[read + 1])
                && IsHexDigit(source[read + 2])
                && IsHexDigit(source[read + 3]);
        }

        public static bool IsValidUTF32(ReadOnlySpan<char> source, int read)
        {
            return IsHexDigit(source[read + 0])
                && IsHexDigit(source[read + 1])
                && IsHexDigit(source[read + 2])
                && IsHexDigit(source[read + 3])
                && IsHexDigit(source[read + 4])
                && IsHexDigit(source[read + 5])
                && IsHexDigit(source[read + 6])
                && IsHexDigit(source[read + 7]);
        }

        public static uint GetUTF16(ReadOnlySpan<char> source, int read)
        {
            var _result = 0U;
            _result += HexToInt(source[read + 0]) << 4 * 3;
            _result += HexToInt(source[read + 1]) << 4 * 2;
            _result += HexToInt(source[read + 2]) << 4 * 1;
            _result += HexToInt(source[read + 3]) << 4 * 0;
            return _result;
        }

        public static uint GetUTF32(ReadOnlySpan<char> source, int read)
        {
            var _result = 0U;
            _result += HexToInt(source[read + 0]) << 4 * 7;
            _result += HexToInt(source[read + 1]) << 4 * 6;
            _result += HexToInt(source[read + 2]) << 4 * 5;
            _result += HexToInt(source[read + 3]) << 4 * 4;
            _result += HexToInt(source[read + 4]) << 4 * 3;
            _result += HexToInt(source[read + 5]) << 4 * 2;
            _result += HexToInt(source[read + 6]) << 4 * 1;
            _result += HexToInt(source[read + 7]) << 4 * 0;
            return _result;
        }

        private static uint HexToInt(char hex)
        {
            // NOTE: uintに変換して
            var _cast = (uint)hex;

            // NOTE: 6シフトすると英字なら1が取得できるので9に変換
            var _mask = (_cast >> 6) * 9;

            // NOTE: 数字なら単に0~9を返し、英字なら0~6+9となる
            return (_cast & 0xF) + _mask;
        }

        private static bool IsHexDigit(uint source)
        {
            return source >= '0' && source <= '9'
                || source >= 'a' && source <= 'f'
                || source >= 'A' && source <= 'F';
        }
    }
}
