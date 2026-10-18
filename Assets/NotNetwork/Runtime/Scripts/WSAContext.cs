using System.Runtime.InteropServices;

namespace NotBura.Packages
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WSAContext
    {
        public ushort Version;
        public ushort HighVersion;
        public ushort MaxSocket;
        public ushort MaxUDPDg;
        public unsafe char* IPVendorInfo;
        public unsafe fixed char Description[256 + 1];
        public unsafe fixed char SystemStatus[128 + 1];
    }
}
