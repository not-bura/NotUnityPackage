using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NotBura.Packages
{
    public static class MethodExtensions
    {
        public static Action ToInvoker<T>(this MethodInfo methodInfo)
        {
            var _parameter = Expression.Parameter(typeof(T));

            return Expression
                .Lambda<Action>(
                    Expression.Call(_parameter, methodInfo),
                    _parameter
                ).Compile();
        }

        public static Action<TArgument> ToInvoke<TSource, TArgument>(this MethodInfo methodInfo)
        {
            var _parameter = Expression.Parameter(typeof(TSource));
            var _argument = Expression.Parameter(typeof(TArgument));

            return Expression
                .Lambda<Action<TArgument>>(
                    Expression.Call(_parameter, methodInfo),
                    _parameter,
                    _argument
                ).Compile();
        }
    }
}
