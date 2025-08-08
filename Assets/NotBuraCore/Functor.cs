using System;

namespace NotBura.Core
{
    public static class Functor<T>
    {
        public static F<T, U> L<U>(Func<T, U> f)
        {
            return new F<T, U>(f);
        }

        public readonly struct F<U, V>
        {
            private readonly Func<T, V> _f;

            public F(Func<T, V> f)
            {
                _f = f;
            }

            public readonly F<V, W> L<W>(Func<V, W> f)
            {
                var a = _f;
                return new F<V, W>(x => f(a(x)));
            }

            public readonly V Do(in T t)
            {
                return _f(t);
            }
        }
    }
}
