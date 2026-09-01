using System;

namespace NotBura.Core
{
    // NOTE: 動的にDBを使用する用の実装
    public sealed class NotDynamicDatabase
        : IDisposable
    {
        #region constructor finalizer

        public NotDynamicDatabase(NotDatabaseSystemHandle handle)
        {

        }

        #endregion constructor finalizer

        #region interface method

        public void Dispose()
        {
        }

        #endregion interface method

        #region public method

        public NotTable<T> CreateTable<T>(ReadOnlySpan<char> name)
            where T : unmanaged
        {
            return new();
        }

        #endregion public method

        #region private method
        #endregion private method
    }
}
