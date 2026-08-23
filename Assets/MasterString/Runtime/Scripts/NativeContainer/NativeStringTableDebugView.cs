#if UNITY_EDITOR
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace NotBura.Packages
{
    internal sealed class NativeStringTableDebugView
    {
        private NativeStringTable m_target;

        public unsafe uint[] Offsets
        {
            get
            {
                ref var target = ref m_target;
                if (target.IsInvalid)
                {
                    return null;
                }

                var length = target.Length;
                var array = new uint[length];
                fixed (void* destination = array)
                {
                    void* source = target.m_buffer;
                    UnsafeUtility.MemCpy(destination, source, length * sizeof(uint));
                }
                return array;
            }
        }

        public unsafe string[] Buffer
        {
            get
            {
                ref var target = ref m_target;
                if (target.IsInvalid)
                {
                    return null;
                }

                var length = target.Length;
                var array = new string[length];
                if (target.IsUTF16)
                {
                    var table = (uint*)target.m_buffer;
                    var buffer = (byte*)((uint*)target.m_buffer + length);

                    var span = array.AsSpan();
                    for (int i = 0, loop = length - 1; i < loop; ++i)
                    {
                        var offset = table[i];
                        var size = (int)(table[i + 1] - offset) >> 1;

                        span[i] = new((char*)(void*)(buffer + offset), 0, size);
                    }

                    {
                        var last = length - 1;
                        var offset = table[last];
                        var size = (int)((target.m_bufferSize - (length * sizeof(uint))) - offset) >> 1;
                        span[last] = new((char*)(void*)(buffer + offset), 0, size);
                    }
                }
                else
                {
                }

                return array;
            }
        }

        public NativeStringTableDebugView(NativeStringTable target)
        {
            m_target = target;
        }
    }
}
#endif
