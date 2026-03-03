using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public static class MasterStringAPI
    {
        private static IMasterStrinProvider<StringIdentifier> s_provider;

        public static IMasterStrinProvider<StringIdentifier> Provider
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => s_provider;
        }

        public static void Register(IMasterStrinProvider<StringIdentifier> provider)
        {
            s_provider = provider;
        }

        public static void Dispose()
        {
            if (s_provider is not null)
            {
                s_provider.Dispose();
                s_provider = null;
            }
        }

        public static void Clear()
        {
            s_provider = null;
        }
    }
}
