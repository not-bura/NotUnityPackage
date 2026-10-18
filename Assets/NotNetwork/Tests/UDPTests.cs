using NUnit.Framework;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace NotBura.Packages
{
    public sealed class UDPTests
    {
        [Test]
        public unsafe void SendTest()
        {
            using var _context = new NativeArray<byte>(sizeof(WSAContext), Allocator.Temp, NativeArrayOptions.ClearMemory);

            using var _handle = NotWSAHandle.Create(_context.GetUnsafePtr());

            if (_handle.IsInvalid)
            {
                Debug.LogError(@$"{_handle.State}
");
            }

            if (NotAddress.TryCreate("127.0.0.1", 5000, ))
            {
                var address =
            }

            if (NotSocket.TryCreate(out var _socket))
            {
                using (_socket)
                {
                    var _buffer = Encoding.UTF8.GetBytes("Hello World!");

                    fixed (void* _pointer = _buffer)
                    {
                        _socket.Send(_pointer, _buffer.Length, );
                    }
                }
            }
        }
    }
}

