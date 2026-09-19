using System;
using System.Collections.Generic;
using Unity.Collections;

namespace NotBura.Packages
{
    public static class NotQueryExtensions
    {
        public static NotQuery<ArrayIterator<T>, T> AsQuery<T>(this T[] source)
        {
            return new(new(source));
        }

        public static NotQuery<ManagedIterator<T>, T> AsQuery<T>(this IEnumerable<T> source)
        {
            return new(new(source));
        }

        public static NotQuery<NativeArrayIterator<T>, T> AsQuery<T>(this in NativeArray<T> source)
            where T : unmanaged
        {
            return new(new(source));
        }

        public static NotQuery<Where<TIterator, T>, T> Where<TIterator, T>(
            this NotQuery<TIterator, T> source
            , Func<T, bool> predicate
        )
            where TIterator : struct, INotQueryIterator<T>
        {
            return new(new(source.Iterator, predicate));
        }

        public static NotQuery<Select<TIterator, TFrom, TTo>, TTo> Select<TIterator, TFrom, TTo>(
            this NotQuery<TIterator, TFrom> source
            , Func<TFrom, TTo> selector
        )
            where TIterator : struct, INotQueryIterator<TFrom>
        {
            return new(new(source.Iterator, selector));
        }

        public static NotQuery<WhereSelect<TIterator, TFrom, TTo>, TTo> Select<TIterator, TFrom, TTo>(
            this NotQuery<Where<TIterator, TFrom>, TFrom> source
            , Func<TFrom, TTo> selector
        )
            where TIterator : struct, INotQueryIterator<TFrom>
        {
            return new(source.Iterator.ToSelect(selector));
        }

        public static NotQuery<Skip<TIterator, T>, T> Skip<TIterator, T>(
            this NotQuery<TIterator, T> source
            , uint count
        )
            where TIterator : struct, INotQueryIterator<T>
        {
            return new(new(source.Iterator, count));
        }

        public static NotQuery<Take<TIterator, T>, T> Take<TIterator, T>(
            this NotQuery<TIterator, T> source
            , uint count
        )
            where TIterator : struct, INotQueryIterator<T>
        {
            return new(new(source.Iterator, count));
        }

        public static NotQuery<SkipTake<TIterator, T>, T> Take<TIterator, T>(
            this NotQuery<Skip<TIterator, T>, T> source
            , uint count
        )
            where TIterator : struct, INotQueryIterator<T>
        {
            return new(source.Iterator.ToTake(count));
        }

        public static T[] ToArray<TIterator, T>(this NotQuery<TIterator, T> source)
            where TIterator : struct, INotQueryIterator<T>
        {
            return ToList(source).ToArray();
        }

        public static List<T> ToList<TIterator, T>(
            this NotQuery<TIterator, T> source
        )
            where TIterator : struct, INotQueryIterator<T>
        {
            var _result = new List<T>();

            var _iterator = source.Iterator;
            while (_iterator.TryMoveNext(out var _current))
            {
                _result.Add(_current);
            }

            return _result;
        }
    }
}
