using System;
using System.Diagnostics;

namespace NotBura.Packages
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HeaderFunctionAttribute : Attribute
    {
        public readonly object[] Arguments;

        public string ItemName { get; set; } = null;
        public bool Parallel { get; set; } = false;
        public bool Editable { get; set; } = false;

        public HeaderFunctionAttribute()
        {
            Arguments = null;
        }

        public HeaderFunctionAttribute(params object[] arguments)
        {
            Arguments = arguments;
        }
    }
}
