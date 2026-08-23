using System.Runtime.CompilerServices;
using System.Text;

namespace NotBura.Packages
{
    public struct NativeString
    {
        private int m_state;
        private unsafe void* m_pointer;

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_state & NativeStringTable.MASK_LENGTH;
        }

        public bool IsUTF16
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (m_state & NativeStringTable.MASK_ENCODE) == 0;
        }

        public bool IsUTF8
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (m_state & NativeStringTable.MASK_ENCODE) != 0;
        }

        internal unsafe NativeString(int state, void* pointer)
        {
            m_state = state;
            m_pointer = pointer;
        }

        public unsafe override string ToString()
        {
            if (IsUTF16)
            {
                return Encoding.Unicode.GetString((byte*)m_pointer, Length);
            }

            var encoding = Encoding.UTF8;
            var count = UTF8ByteCount(m_pointer, Length);
            return encoding.GetString((byte*)m_pointer, count);
        }

        private unsafe int UTF8ByteCount(void* source, int length)
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
    }
}
