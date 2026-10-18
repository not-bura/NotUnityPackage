using System.Runtime.InteropServices;

namespace NotBura.Packages
{
    internal static class Plugin
    {
        private const string NAME = "NotBura.Packages.NotNetwork.Plugin";

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int WSAOpen(
            void* result
        );

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void WSAClose();

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* SocketOpenUDP();

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int SocketClose(
            void* socket
        );

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int SocketAddress(
            void* address
            , ushort port
            , void* buffer
        );

        [DllImport(NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int SocketSendTo(
            void* socket
            , void* buffer
            , int length
            , void* address
        );
    }
}
