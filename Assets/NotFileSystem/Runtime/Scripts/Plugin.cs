using System;
using System.Runtime.InteropServices;

namespace NotBura.Packages
{
    internal static class Plugin
    {
        private const string NAME = "NotBura.Packages.NotFileSystem.Plugin";

        public enum Access
            : uint
        {
            Read    = 0x80000000,
            Write   = 0x40000000,
            Execute = 0x20000000,
            All     = 0x10000000,
        }

        [Flags]
        public enum Share
            : uint
        {
            None    = 0,
            Read    = 0x00000001,
            Write   = 0x00000002,
            Delete  = 0x00000004,
        }

        public enum Mode
        {
            New                 = 1,
            CreateAlways        = 2,
            OpenExisting        = 3,
            OpenAlways          = 4,
            TruncateExisting    = 5,
        }

        public enum Attribute
            : uint
        {
            Readonly            = 0x00000001,
            Hidden              = 0x00000002,
            System              = 0x00000004,
            Directory           = 0x00000010,
            Archive             = 0x00000020,
            Device              = 0x00000040,
            Normal              = 0x00000080,
            Temporary           = 0x00000100,
            SparseFile          = 0x00000200,
            RepaesePoint        = 0x00000400,
            Compressed          = 0x00000800,
            Offline             = 0x00001000,
            NotContentIndexed   = 0x00002000,
            Encrypted           = 0x00004000,
            IntegrityStream     = 0x00008000,
            Virtual             = 0x00010000,
            NoScrubData         = 0x00020000,
            EA                  = 0x00040000,
            Pinned              = 0x00080000,
            UnPinned            = 0x00100000,
            RecallOnOpen        = 0x00040000,
            RecallOnDataAccess  = 0x00400000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OpenContext
        {
            public Access Access;
            public Share Share;
            public Mode Mode;
            public Attribute Attribute;
        }

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void* FileOpenUTF16(char* path, OpenContext* context);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void* FileOpenReadUTF16(char* path);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void* FileOpenWriteUTF16(char* path);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern long FileSize(void* handle);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void FileClose(void* handle);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern uint FileRead(void* handle, void* buffer, uint length);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern uint FileWrite(void* handle, void* buffer, uint length);

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern bool FileFlush(void* handle);
    }
}
