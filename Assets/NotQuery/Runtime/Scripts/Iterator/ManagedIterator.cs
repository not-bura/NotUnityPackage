using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    internal struct ManagedContext
    {
        public object Source;
        public uint State;
        public ManagedContext(object source)
        {
            Source = source;
            State = 0;
        }
    }

    public struct ManagedIterator<T>
        : INotQueryIterator<T>
    {
        private unsafe readonly delegate*<ref ManagedContext, ref T, bool> m_iterator;
        private ManagedContext m_context;

        public unsafe ManagedIterator(IEnumerable<T> source)
        {
            m_iterator = source switch
            {
                T[]                 => &ArrayIterator,
                List<T>             => &ListIterator,
                IList<T>            => &IListIterator,
                IReadOnlyList<T>    => &IReadOnlyListIterator,
                _                   => &EnumerableIterator,
            };

            m_context = new(source);
        }

        public unsafe bool TryMoveNext(out T current)
        {
            Unsafe.SkipInit(out current);
            return m_iterator(ref m_context, ref current);
        }

        private static bool ArrayIterator(ref ManagedContext context, ref T result)
        {
            var _cast = Unsafe.As<T[]>(context.Source);

            if (context.State >= (uint)_cast.Length)
            {
                return false;
            }

            result = _cast[context.State];
            ++context.State;
            return true;
        }

        private static bool ListIterator(ref ManagedContext context, ref T result)
        {
            var _cast = Unsafe.As<List<T>>(context.Source);

            if (context.State >= (uint)_cast.Count)
            {
                return false;
            }

            result = _cast[unchecked((int)context.State)];
            ++context.State;
            return true;
        }

        private static bool IListIterator(ref ManagedContext context, ref T result)
        {
            var _cast = Unsafe.As<IList<T>>(context.Source);

            if (context.State >= (uint)_cast.Count)
            {
                return false;
            }

            result = _cast[unchecked((int)context.State)];
            ++context.State;
            return true;
        }

        private static bool IReadOnlyListIterator(ref ManagedContext context, ref T result)
        {
            var _cast = Unsafe.As<IReadOnlyList<T>>(context.Source);

            if (context.State >= (uint)_cast.Count)
            {
                return false;
            }

            result = _cast[unchecked((int)context.State)];
            ++context.State;
            return true;
        }

        private static bool EnumerableIterator(ref ManagedContext context, ref T result)
        {
            var _enumrator = Get(ref context);

            while (_enumrator.MoveNext())
            {
                result = _enumrator.Current;
                return true;
            }

            return false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static IEnumerator<T> Get(ref ManagedContext context)
            {
                if (context.State is not 0)
                {
                    return Unsafe.As<IEnumerator<T>>(context.Source);
                }

                var enumerator = Unsafe.As<IEnumerable<T>>(context.Source).GetEnumerator();
                context.Source = enumerator;
                context.State = 1;

                return enumerator;
            }
        }
    }
}
