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
            private static readonly Func<List<T>, T[]> s_getter = CreateGetter();

            private static readonly FieldInfo s_fieldInfo = CreateFieldInfo();

            public static Func<List<T>, T[]> Getter
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => s_getter;
            }

            public static FieldInfo FieldInfo
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => s_fieldInfo;
            }

            private static Func<List<T>, T[]> CreateGetter()
            {
                var _fieldInfo = CreateFieldInfo();

                return _fieldInfo.ToGetter<List<T>, T[]>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static FieldInfo CreateFieldInfo()
            {
                var _fieldInfo = typeof(List<T>)
                    .GetField(s_name, BindingFlags.NonPublic | BindingFlags.Instance);

                return _fieldInfo;
            }
        }

        // NOTE: 毎度GetField"s"しない為の名前のキャッシュ
        // static string確保してしまうがしょうがない
        private static readonly string s_name = GetFieldName();

        #region AsSpanTemporary Method

        // NOTE: Cache<T>を通さないので呼ぶ回数が極端に少ないならメモリと速度の観点で得になる(はず)
        public static Span<T> AsSpanTemporary<T>(this List<T> source)
        {
            var _array = GetArrayTemporary(source);
            return _array.AsSpan(0, source.Count);
        }

        public static Span<T> AsSpanTemporary<T>(this List<T> source, int start)
        {
            var _array = GetArrayTemporary(source);
            return _array.AsSpan(start, source.Count - start);
        }

        public static Span<T> AsSpanTemporary<T>(this List<T> source, int start, int length)
        {
            var _array = GetArrayTemporary(source);
            return _array.AsSpan(start, length);
        }

        public static Span<T> AsSpanTemporary<T>(this List<T> source, Index index)
        {
            var _array = GetArrayTemporary(source);
            return _array.AsSpan(index);
        }

        public static Span<T> AsSpanTemporary<T>(this List<T> source, Range range)
        {
            var _array = GetArrayTemporary(source);
            return _array.AsSpan(range);
        }

        #endregion AsSpanTemporary Method

        #region AsSpan Method

        public static Span<T> AsSpan<T>(this List<T> source)
        {
            var _array = GetArray(source);
            return _array.AsSpan(0, source.Count);
        }

        public static Span<T> AsSpan<T>(this List<T> source, int start)
        {
            var _array = GetArray(source);
            return _array.AsSpan(start, source.Count - start);
        }

        public static Span<T> AsSpan<T>(this List<T> source, int start, int length)
        {
            var _array = GetArray(source);
            return _array.AsSpan(start, length);
        }

        public static Span<T> AsSpan<T>(this List<T> source, Index index)
        {
            var _array = GetArray(source);
            return _array.AsSpan(index);
        }

        public static Span<T> AsSpan<T>(this List<T> source, Range range)
        {
            var _array = GetArray(source);
            return _array.AsSpan(range);
        }

        #endregion AsSpan Method

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] GetArrayTemporary<T>(List<T> source)
        {
            var _fieldInfo = typeof(List<T>)
                .GetField(s_name, BindingFlags.NonPublic | BindingFlags.Instance);
            var _result = (T[])_fieldInfo.GetValue(source);
            return _result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] GetArray<T>(List<T> source)
        {
            var _array =
#if ENABLE_LIST_EXTENSIONS_REFLECTION
                (T[])Cache<T>.FieldInfo.GetValue(source);
#else
                Cache<T>.Getter(source);
#endif
            return _array;
        }
    }
}
