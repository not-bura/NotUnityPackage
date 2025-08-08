using Microsoft.Win32.SafeHandles;
using System;
using System.IO;

namespace NotBura.Core
{
    // NOTE: SafeFileHandleと同じくDB抽象化用ハンドル
    public sealed class NotDatabaseSystemHandle
        : IDisposable
    {
        private SafeFileHandle m_fileHandle;

        #region constructor finalizer

        public NotDatabaseSystemHandle(ReadOnlySpan<char> path)
        {
            // TODO: stringを通さない実装を模索する
            var marshal = new string(path);
            Construct(marshal);
        }

        // TODO: stringを通さない実装を模索する
        private void Construct(string path)
        {
            FileStream fs = null;
            try
            {
                fs = File.Open(
                    path
                    , FileMode.OpenOrCreate
                    , FileAccess.Read
                    , FileShare.ReadWrite
                );
            }
            catch
            {
                if (fs is not null)
                {
                    fs.Dispose();
                    fs = null;
                }
            }

            if (fs is null)
            {
                return;
            }

            m_fileHandle = fs.SafeFileHandle;
        }

        ~NotDatabaseSystemHandle()
        {
            Dispose(false);
        }

        #endregion constructor finalizer

        #region interface method

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion interface method

        #region private method

        private void Dispose(bool disposing)
        {
            if (m_fileHandle is not null)
            {
                m_fileHandle.Dispose();
                m_fileHandle = null;
            }
        }

        #endregion private method
    }
}
