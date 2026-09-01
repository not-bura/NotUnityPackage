using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace NotBura.Packages
{
    [NativeContainer]
#if UNITY_EDITOR
    [DebuggerDisplay("Size = {m_size}")]
#endif
    public struct NativeBuffer
        : IDisposable
    {
        internal unsafe void* m_buffer;
        internal long m_size;


        internal Allocator m_AllocatorLabel;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
        private static int s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeBuffer>();
#endif

        public unsafe NativeBuffer(long size, Allocator allocator, NativeArrayOptions option)
        {
            m_buffer = UnsafeUtility.MallocTracked(size, UnsafeUtility.AlignOf<byte>(), allocator, 0);
            m_size = size;
            m_AllocatorLabel = allocator;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = CollectionHelper.CreateSafetyHandle(allocator);
            AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, s_staticSafetyId);
#endif
        }

        public unsafe void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (m_AllocatorLabel == Allocator.Invalid && !AtomicSafetyHandle.IsDefaultValue(m_Safety))
            {
                AtomicSafetyHandle.CheckExistsAndThrow(m_Safety);
            }
#endif

            if (m_AllocatorLabel > Allocator.None)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                CollectionHelper.DisposeSafetyHandle(ref m_Safety);
#endif
                UnsafeUtility.FreeTracked(m_buffer, m_AllocatorLabel);
                m_AllocatorLabel = Allocator.Invalid;
            }

            m_buffer = null;
        }
    }
}
