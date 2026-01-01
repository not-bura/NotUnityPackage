using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public static class ListExtensions
    {
        /// <summary>
        /// Generic type cachingパターンを用いるキャッシュクラス
        /// </summary>
        private class Cache<T>
        {
            // NOTE: getterの生成は重たいのでstatic field constructorで安全にキャッシュ
            private static readonly Func<List<T>, T[]> s_getter = Get();

            public static Func<List<T>, T[]> Gettor
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => s_getter;
            }

            private static Func<List<T>, T[]> Get()
            {
                var _fieldInfo = typeof(List<T>)
                    .GetField(s_name, BindingFlags.NonPublic | BindingFlags.Instance);
                return _fieldInfo.ToGetter<List<T>, T[]>();
            }
        }

        // NOTE: 毎度GetField"s"しない為の名前のキャッシュ
        // static string確保してしまうがしょうがない
        private static readonly string s_name = GetFieldName();

        private static string GetFieldName()
        {
            var _fields = typeof(List<>).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in _fields)
            {
                if (field.FieldType.IsArray)
                {
                    return field.Name;
                }
            }

            return null;
        }

        public static Span<T> AsSpanTemporary<T>(this List<T> source)
        {
            var _fieldInfo = typeof(List<T>)
                .GetField(s_name, BindingFlags.NonPublic | BindingFlags.Instance);
            var _result = (T[])_fieldInfo.GetValue(source);
            return _result.AsSpan();
        }

        public static Span<T> AsSpan<T>(this List<T> source)
        {
            return Cache<T>.Gettor(source).AsSpan(0, source.Count);
        }

        public static Span<T> AsSpan<T>(this List<T> source, int start)
        {
            return Cache<T>.Gettor(source).AsSpan(start, source.Count - start);
        }

        public static Span<T> AsSpan<T>(this List<T> source, int start, int length)
        {
            return Cache<T>.Gettor(source).AsSpan(start, length);
        }

        public static Span<T> AsSpan<T>(this List<T> source, Index index)
        {
            return Cache<T>.Gettor(source).AsSpan(index);
        }

        public static Span<T> AsSpan<T>(this List<T> source, Range range)
        {
            return Cache<T>.Gettor(source).AsSpan(range);
        }
    }
}
