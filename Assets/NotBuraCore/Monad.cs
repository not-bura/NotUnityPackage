using System;

namespace NotBura.Core
{
    public static class Monad
    {
        public static F<U> L<T, U>(this T t, Func<T, U> f)
        {
            return new F<U>(() => f(t));
        }

        public static F<T> L<T>(Func<Unit, T> f)
        {
            return new F<T>(() => f(new()));
        }

        public struct Unit
        {
        }

        public readonly struct F<T>
        {
            private readonly Func<T> _f;

            public F(Func<T> f)
            {
                _f = f;
            }

            public readonly F<U> L<U>(Func<T, U> f)
            {
                var a = _f;
                return new F<U>(() => f(a()));
            }

            public readonly T Do()
            {
                return _f();
            }
        }
    }
}
