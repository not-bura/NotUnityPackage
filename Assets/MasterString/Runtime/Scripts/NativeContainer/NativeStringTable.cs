using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace NotBura.Packages
{
    public enum NativeStringEncodeTypes
    {
        UTF16,
        UTF8,
    }

    [NativeContainer]
    [NativeContainerIsReadOnly]
#if UNITY_EDITOR
    [DebuggerDisplay("Encode = {(m_state & 0x80_00_00_00) == 0 ? \"UTF16\" : \"UTF8\"} Length = {m_state & 0x7F_FF_FF_FF}")]
    [DebuggerTypeProxy(typeof(NativeStringTableDebugView))]
#endif
    public struct NativeStringTable
    : IDisposable
    {
        internal const int MASK_LENGTH = 0x7F_FF_FF_FF;
        internal const int MASK_ENCODE = unchecked((int)0x80_00_00_00);

        internal int m_state;
        internal long m_bufferSize;

        [NativeDisableUnsafePtrRestriction]
        internal unsafe void* m_buffer;

        // NOTE: "m_AllocatorLabel"固定である必要がある
        internal Allocator m_AllocatorLabel;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        // NOTE: "m_Safety"固定である必要がある
        internal AtomicSafetyHandle m_Safety;
        private static int s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeStringTable>();
#endif

        public unsafe bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_buffer != null;
        }

        public unsafe bool IsInvalid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_buffer == null;
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_state & MASK_LENGTH;
        }

        public bool IsUTF16
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (m_state & MASK_ENCODE) == 0;
        }

        public bool IsUTF8
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_state < 0;
        }

        public unsafe NativeString this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                var length = Length;

                if ((uint)index >= (uint)length)
                {
                    throw new IndexOutOfRangeException(nameof(index));
                }

                if (index == length - 1)
                {

                }

                var table = (uint*)m_buffer;
                var offset = table[index];
                var state = (int)(table[index + 1] - offset);

                if (IsUTF8)
                {
                    state |= MASK_ENCODE;
                }

                var pointer = (byte*)((uint*)m_buffer + length)[offset];

                return new(state, pointer);
            }
        }

        internal unsafe NativeStringTable(int state, long bufferSize, void* buffer, Allocator allocator)
        {
            m_state = state;
            m_bufferSize = bufferSize;
            m_buffer = buffer;
            m_AllocatorLabel = allocator;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, s_staticSafetyId);
#endif
        }

        public unsafe void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (m_AllocatorLabel != Allocator.None && false == AtomicSafetyHandle.IsDefaultValue(m_Safety))
            {
                AtomicSafetyHandle.CheckExistsAndThrow(m_Safety);
            }
#endif

            if (IsInvalid)
            {
                return;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (m_AllocatorLabel == Allocator.Invalid)
            {
                throw new InvalidOperationException("The NativeArray can not be Disposed because it was not allocated with a valid allocator.");
            }

            if (m_AllocatorLabel >= Allocator.FirstUserIndex)
            {
                throw new InvalidOperationException("The NativeArray can not be Disposed because it was allocated with a custom allocator, use CollectionHelper.Dispose in com.unity.collections package.");
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

        public unsafe static NativeStringTable FromSource(string[] source, Allocator allocator, NativeStringEncodeTypes encode = NativeStringEncodeTypes.UTF16)
        {
            var state = source.Length;
            var bufferSize = (long)source.Length * sizeof(uint);

            if (encode == NativeStringEncodeTypes.UTF16)
            {
                var span = source.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    bufferSize += unchecked((uint)span[i].Length * sizeof(char));
                }

                var buffer = UnsafeUtility.MallocTracked(bufferSize, UnsafeUtility.AlignOf<byte>(), allocator, 0);

                var byteOffset = 0U;
                var charOffset = 0U;
                var offsetBegin = (uint*)buffer;
                var bufferBigin = (char*)(void*)((uint*)buffer + span.Length);

                for (int i = 0; i < span.Length; ++i)
                {
                    var text = span[i];

                    var length = unchecked((uint)text.Length);
                    var size = unchecked(length * sizeof(char));

                    var lhs = bufferBigin + charOffset;
                    fixed (void* rhs = text)
                    {
                        UnsafeUtility.MemCpy(lhs, rhs, size);
                    }

                    charOffset += length;

                    offsetBegin[i] = byteOffset;
                    byteOffset += size;
                }

                return new(state, bufferSize, buffer, allocator);
            }
            else
            {
                state |= MASK_ENCODE;

                var encoding = Encoding.UTF8;
                var span = source.AsSpan();
                for (int i = 0; i < span.Length; ++i)
                {
                    bufferSize += encoding.GetByteCount(span[i]);
                }

                var buffer = UnsafeUtility.MallocTracked(bufferSize, UnsafeUtility.AlignOf<byte>(), allocator, 0);

                return new(state, bufferSize, buffer, allocator);
            }
        }
    }
}
