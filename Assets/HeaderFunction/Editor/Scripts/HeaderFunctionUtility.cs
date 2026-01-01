using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public class HeaderFunctionUtility
    {
        public static void SetResultHandler(Action<object> handler)
        {
            HeaderFunctionHandler.ResultHandler = handler;
        }

        public static bool IsAsync(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }

        public static bool IsNullable(Type type)
        {
            if (false == type.IsValueType)
            {
                return true;
            }

            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
