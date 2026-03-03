using System;

namespace NotBura.Packages
{
    public interface IMasterStrinProvider<T>
        : IDisposable
    {
        public ReadOnlySpan<char> Resolve(T source);
    }
}
