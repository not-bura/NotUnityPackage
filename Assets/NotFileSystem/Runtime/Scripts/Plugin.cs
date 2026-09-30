using System.Runtime.InteropServices;

namespace NotBura.Packages
{
    public static class Plugin
    {
        private const string NAME = "NotBura.Packages.NotFileSystem.Plugin";

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern long FileSize(void* handle);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void* FileOpenUTF16(char* path);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void* FileOpenReadUTF16(char* path);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void* FileOpenWriteUTF16(char* path);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void FileClose(void* handle);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern uint FileWrite(void* handle, void* buffer, uint length);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern uint FileRead(void* handle, void* buffer, uint length);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern bool FileFlush(void* handle);
    }
}
