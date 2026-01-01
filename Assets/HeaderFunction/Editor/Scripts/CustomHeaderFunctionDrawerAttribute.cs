using System;

namespace NotBura.Packages
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CustomHeaderFunctionDrawerAttirbute
        : Attribute
    {
        public readonly Type Type;

        public CustomHeaderFunctionDrawerAttirbute(Type type)
        {
            Type = type;
        }
    }
}

