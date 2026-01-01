using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NotBura.Packages
{
    public class AwaitableFactory
    {
        private const string NAME_OF_GET_AWAITER    = "GetAwaiter";
        private const string NAME_OF_IS_COMPLETED   = "IsCompleted";
        private const string NAME_OF_GET_RESULT     = "GetResult";
        private const BindingFlags AWAITABLE_PATTERN_FLAGS = BindingFlags.Public | BindingFlags.Instance;

        private MethodInfo m_getAwaiterMethodInfo;
        private Type m_awaiterType;
        // NOTE: IsCompletedは終了確認の為大量に呼ばれるのでLamdaで高速化する
        private Func<object, bool> m_isCompleted;
        private MethodInfo m_getResultMethodInfo;

        public AwaitableFactory()
        {
        }

        public AwaitableWrapper GetWrapper(object source)
        {
            var _awaiter = GetAwaiter(source);
            m_awaiterType = _awaiter.GetType();
            return new(_awaiter, this);
        }

        private object GetAwaiter(object source)
        {
            if (m_getAwaiterMethodInfo == null)
            {
                m_getAwaiterMethodInfo = source
                    .GetType()
                    .GetMethod(NAME_OF_GET_AWAITER, AWAITABLE_PATTERN_FLAGS);
            }

            return m_getAwaiterMethodInfo.Invoke(source, null);
        }

        public bool IsCompleted(object source)
        {
            if (m_isCompleted == null)
            {
                var _isCompletedPropertyInfo = m_awaiterType
                    .GetProperty(NAME_OF_IS_COMPLETED, AWAITABLE_PATTERN_FLAGS);

                // NOTE: (bool)_protpertyInfo.GetValue(source, null)をFuncで構築する
                var _parameter = Expression.Parameter(typeof(object));
                var _lamda = Expression.Lambda<Func<object, bool>>(
                    Expression.Property(
                        Expression.Convert(_parameter, m_awaiterType),
                        _isCompletedPropertyInfo
                    ),
                    _parameter
                );

                m_isCompleted = _lamda.Compile();
            }

            return m_isCompleted(source);
        }

        public bool IsVoidResult()
        {
            if (m_getResultMethodInfo == null)
            {
                m_getResultMethodInfo = m_awaiterType
                    .GetMethod(NAME_OF_GET_RESULT, AWAITABLE_PATTERN_FLAGS);
            }

            var _type = m_getResultMethodInfo.ReturnType;
            return _type == typeof(void)
                || _type.FullName == "System.Threading.Tasks.VoidTaskResult";
        }

        public object GetResult(object source)
        {
            if (m_getResultMethodInfo == null)
            {
                m_getResultMethodInfo = m_awaiterType
                    .GetMethod(NAME_OF_GET_RESULT, AWAITABLE_PATTERN_FLAGS);
            }

            return m_getResultMethodInfo.Invoke(source, null);
        }
    }
}
