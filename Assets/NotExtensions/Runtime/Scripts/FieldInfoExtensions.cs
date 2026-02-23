using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NotBura.Packages
{
    public static class FieldInfoExtensions
    {
        public static Func<TSource, TResult> ToGetter<TSource, TResult>(this FieldInfo fieldInfo)
        {
            var _parameter = Expression.Parameter(typeof(TSource));

            var _field = Expression.Field(
                fieldInfo.IsStatic
                    ? null
                    : _parameter,
                fieldInfo
            );

            return Expression
                .Lambda<Func<TSource, TResult>>(_field, _parameter)
                .Compile();
        }

        public static Func<object, T> ToGetterTypeToBoxed<T>(this FieldInfo fieldInfo)
        {
            var _parameter = Expression.Parameter(typeof(object));

            var _field = Expression.Field(
                fieldInfo.IsStatic
                    ? null
                    : Expression.Convert(_parameter, fieldInfo.DeclaringType),
                fieldInfo
            );

            return Expression
                .Lambda<Func<object, T>>(_field, _parameter)
                .Compile();
        }

        public static Func<T, object> ToGetterBoxedToType<T>(this FieldInfo fieldInfo)
        {
            var _parameter = Expression.Parameter(typeof(object));

            var _field = Expression.Field(
                fieldInfo.IsStatic
                    ? null
                    : _parameter,
                fieldInfo
            );

            return Expression
                .Lambda<Func<T, object>>(
                    Expression.Convert(_field, typeof(object)),
                    _parameter
                )
                .Compile();
        }

        public static Func<object, object> ToGetterBoxedToBoxed(this FieldInfo fieldInfo)
        {
            var _parameter = Expression.Parameter(typeof(object));

            var _field = fieldInfo.IsStatic
                ? null
                : Expression.Field(
                    Expression.Convert(_parameter, fieldInfo.DeclaringType),
                    fieldInfo
                );

            return Expression
                .Lambda<Func<object, object>>(
                    Expression.Convert(_field, typeof(object)),
                    _parameter
                )
                .Compile();
        }

        public static Action<TSource, TValue> ToSetter<TSource, TValue>(this FieldInfo fieldInfo)
        {
            var _source = Expression.Parameter(typeof(TSource));
            var _value = Expression.Parameter(typeof(TValue));

            var _field = Expression.Field(
                fieldInfo.IsStatic
                    ? null
                    : _source,
                fieldInfo
            );

            return Expression
                .Lambda<Action<TSource, TValue>>(
                    Expression.Assign(_field, _value),
                    _source,
                    _value
                )
                .Compile();
        }

        public static Action<object, T> ToSetter<T>(this FieldInfo fieldInfo)
        {
            var _target = Expression.Parameter(typeof(object));
            var _value = Expression.Parameter(typeof(T));

            return Expression.Lambda<Action<object, T>>(
                Expression.Assign(
                    Expression.Field(
                        Expression.Convert(_target, fieldInfo.DeclaringType),
                        fieldInfo
                    ),
                    Expression.Convert(_value, fieldInfo.FieldType)
                ),
                _target,
                _value
            ).Compile();
        }
    }
}
