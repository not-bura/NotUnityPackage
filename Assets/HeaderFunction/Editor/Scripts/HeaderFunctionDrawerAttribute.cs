using System;

namespace NotBura.Packages
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HeaderFunctionDrawerAttirbute
        : Attribute
    {
        public readonly Type Type;

        public HeaderFunctionDrawerAttirbute(Type type)
        {
            Type = type;
        }
    }
}

