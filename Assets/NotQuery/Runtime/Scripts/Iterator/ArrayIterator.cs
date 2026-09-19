using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public struct ArrayIterator<T>
        : INotQueryIterator<T>
    {
        private T[] m_source;
        private uint m_index;

        public ArrayIterator(T[] source)
        {
            m_source = source;
            m_index = 0;
        }

        public unsafe bool TryMoveNext(out T current)
        {
            if (m_index >= (uint)m_source.Length)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            current = m_source[m_index];
            ++m_index;
            return true;
        }
    }
}
