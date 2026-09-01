using System.Text;

namespace NotBura.Packages
{
    public static class UTF8Helper
    {
        private static unsafe int UTF8CharCount(void* source, int length)
        {
            const ulong LENGTH_TABLE = 0x_4322_1111_1111_1111UL;

            var index = 0;
            var offset = 0UL;
            var pointer = (byte*)source;

            while (index < length)
            {
                ++index;
                var nibble = *(pointer + offset) >> 4;
                var count = (LENGTH_TABLE >> (nibble * 4)) & 0x0F;

                offset += count;
            }

            return index;
        }

        public static unsafe uint GetByteCount(string[] source)
        {
            var result = 0U;

            for (int i = 0; i < source.Length; ++i)
            {
                fixed (char* pointer = source[i])
                {
                    for (int j = 0; j < source[i].Length; ++j)
                    {
                        if (pointer[j] <= 0x007F)
                        {
                            result += 1;
                            continue;
                        }

                        if (pointer[j] <= 0x07FF)
                        {
                            result += 2;
                            continue;
                        }

                        if (pointer[j] < 0xD800 || pointer[j] > 0xDFFF)
                        {
                            result += 3;
                            continue;
                        }

                        result += 4;
                        ++j;
                    }
                }
            }

            return result;
        }

        public static uint GetByteCountTrue(string[] source)
        {
            var result = 0U;
            var encoding = Encoding.UTF8;

            for (int i = 0; i < source.Length; ++i)
            {
                result += (uint)encoding.GetByteCount(source[i]);
            }

            return result;
        }

        public static unsafe uint GetByteCount(char* source, uint length)
        {
            var result = 0U;

            return result;
        }
    }
}
