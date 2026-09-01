using System.Runtime.CompilerServices;

namespace NotBura.Core
{
    // NOTE: ポインターを安全に使うためのハンドル参照構造体
    public unsafe readonly ref struct NotHandle<T>
        where T : unmanaged
    {
        private readonly T* m_pointer;

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => *m_pointer;
        }

        public NotHandle(T* pointer)
        {
            m_pointer = pointer;
        }

        public static implicit operator T(NotHandle<T> handle)
        {
            return *handle.m_pointer;
        }
    }
}
