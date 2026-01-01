using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NotBura.Packages
{
    public static class PropertyExtensions
    {
        public static Func<TSource, TResult> ToGetter<TSource, TResult>(this PropertyInfo propertyInfo)
        {
            var _parameter = Expression.Parameter(typeof(TSource));

            var _property = Expression.Property(
                propertyInfo.GetMethod.IsStatic
                    ? null
                    : _parameter,
                propertyInfo
            );

            return Expression
                .Lambda<Func<TSource, TResult>>(_property, _parameter)
                .Compile();
        }

        public static Func<object, T> ToGetterBoxedToType<T>(this PropertyInfo propertyInfo)
        {
            var _parameter = Expression.Parameter(typeof(object));

            var _property = Expression.Property(
                propertyInfo.GetMethod.IsStatic
                    ? null
                    : Expression.Convert(_parameter, propertyInfo.DeclaringType),
                propertyInfo
            );

            return Expression
                .Lambda<Func<object, T>>(_property, _parameter)
                .Compile();
        }

        public static Action<TSource, TValue> ToSetter<TSource, TValue>(this PropertyInfo propertyInfo)
        {
            var _parameter = Expression.Parameter(typeof(TSource));
            var _value = Expression.Parameter(typeof(TValue));

            var _property = Expression.Property(
                propertyInfo.GetMethod.IsStatic
                    ? null
                    : _parameter,
                propertyInfo
            );

            return Expression
                .Lambda<Action<TSource, TValue>>(
                    Expression.Assign(_property, _value),
                    _parameter,
                    _value
                )
                .Compile();
        }
    }
}
