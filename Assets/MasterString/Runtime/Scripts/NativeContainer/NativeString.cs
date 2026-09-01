using System.Runtime.CompilerServices;
using System.Text;
using static NotBura.Packages.NativeStringTable;

namespace NotBura.Packages
{
    public struct NativeString
    {
        private int m_state;
        private unsafe void* m_pointer;

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_state & MASK_LENGTH;
        }

        public bool IsUTF16
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (m_state & MASK_ENCODE) == 0;
        }

        public bool IsUTF8
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_state < 0;
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
                return new((char*)m_pointer, 0, Length >> 1);
            }

            return Encoding.UTF8.GetString((byte*)m_pointer, Length);
        }
    }
}
