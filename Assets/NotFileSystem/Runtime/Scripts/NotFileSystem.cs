namespace NotBura.Packages
{
    public static class NotFileSystem
    {
        public static unsafe NotFileWriter OpenWrite(string path)
        {
            if (path is null or { Length: 0 })
            {
                return default;
            }

            fixed (char* _pointer = path)
            {
                var _handle = Plugin.FileOpenWriteUTF16(_pointer);
                return new(_handle);
            }
        }

        public static unsafe NotFileReader OpenRead(string path)
        {
            if (path is null or { Length: 0 })
            {
                return default;
            }

            fixed (char* _pointer = path)
            {
                var _handle = Plugin.FileOpenReadUTF16(_pointer);
                return new(_handle);
            }
        }
    }
}
