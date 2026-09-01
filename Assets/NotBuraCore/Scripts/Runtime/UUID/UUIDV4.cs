using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace NotBura.Core
{
#if UNITY_EDITOR
    [DebuggerDisplay("{ToString()}")]
#endif
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct UUIDV4
        : IUUID
        , IComparable<UUIDV4>
        , IEquatable<UUIDV4>
    {
        [SerializeField] private ulong m_high;
        [SerializeField] private ulong m_low;

        public unsafe ref UUID ToUUID()
        {
            fixed (void* ptr = &this)
            {
                return ref *(UUID*)ptr;
            }
        }

        #region interface method

        public unsafe bool Equals(UUIDV4 other)
        {
            fixed (void* pointer = &this)
            {
                return UnsafeUtility.MemCmp(pointer, &other, 16) == 0;
            }
        }

        public unsafe int CompareTo(UUIDV4 other)
        {
            fixed (void* pointer = &this)
            {
                return UnsafeUtility.MemCmp(pointer, &other, 16);
            }
        }

        #endregion interface method

        #region override method

        [Obsolete("Call boxing method.")]
#pragma warning disable CS0809
        public override bool Equals(object obj)
#pragma warning restore CS0809
        {
            return obj is UUIDV4 cast && Equals(cast);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_high, m_low);
        }

        public override unsafe string ToString()
        {
            fixed (void* pointer = &this)
            {
                return IUUID.ToString(pointer);
            }
        }

        #endregion override method

        #region implicit operator

        public static implicit operator UUID(UUIDV4 other)
        {
            // NOTE: unsafeコンテキストで直接ポインタ操作しても速度差がほぼなかった
            //unsafe
            //{
            //    return *(UUID*)((void*)&other);
            //}

            return UnsafeUtility.As<UUIDV4, UUID>(ref other);
        }

        public static implicit operator UUIDV4(UUID other)
        {
            return UnsafeUtility.As<UUID, UUIDV4>(ref other);
        }

        #endregion implicit operator
    }
}
