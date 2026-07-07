using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public static class MasterStringAPI
    {
        private static IMasterStrinProvider<StringIdentifier> s_masterStringProvider;
        private static IMasterStrinProvider<StringIdentifier> s_valueStringProvider;

        // TODO: エディタ上で表示する用で一旦用意したが必要ない気がする
        private static IMasterStrinProvider<StringIdentifier> s_editorOnlyStringProvider;

        public static IMasterStrinProvider<StringIdentifier> MasterStringProvider
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => s_masterStringProvider;
        }

        public static IMasterStrinProvider<StringIdentifier> ValueStringProvider
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => s_valueStringProvider;
        }

        public static void RegisterMasterStringProvider(IMasterStrinProvider<StringIdentifier> value)
        {
            s_masterStringProvider = value;
        }

        public static void RegisterValueStringProvider(IMasterStrinProvider<StringIdentifier> value)
        {
            s_valueStringProvider = value;
        }

        public static void Dispose()
        {
            if (s_masterStringProvider is not null)
            {
                s_masterStringProvider.Dispose();
                s_masterStringProvider = null;
            }

            if (s_valueStringProvider is not null)
            {
                s_masterStringProvider.Dispose();
                s_valueStringProvider = null;
            }
        }

        public static void DisposeMasterStringProvider()
        {
            if (s_masterStringProvider is null)
            {
                return;
            }

            s_masterStringProvider.Dispose();
            s_masterStringProvider = null;
        }

        public static void DisposeValueStringProvider()
        {
            if (s_valueStringProvider is null)
            {
                return;
            }

            s_valueStringProvider.Dispose();
            s_valueStringProvider = null;
        }
    }
}
