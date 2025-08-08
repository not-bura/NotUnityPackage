using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace NotBura.Core
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct UUIDV4
        : IUUID
        , IComparable<UUIDV4>
        , IEquatable<UUIDV4>
    {
        [SerializeField] private ulong m_high;
        [SerializeField] private ulong m_low;

        public unsafe ref UUID ToMarshalUUID()
        {
            fixed (void* ptr = &this)
            {
                return ref *(UUID*)ptr;
            }
        }

        #region interface method

        public readonly bool Equals(UUIDV4 other)
        {
            return IUUID.Equals(m_high, m_low, other.m_high, other.m_low);
        }

        public readonly int CompareTo(UUIDV4 other)
        {
            return IUUID.CompareTo(m_high, m_low, other.m_high , other.m_low);
        }

        #endregion interface method

        #region override method

        [Obsolete("Call boxing method.")]
#pragma warning disable CS0809
        public override readonly bool Equals(object obj)
#pragma warning restore CS0809
        {
            return obj is UUIDV4 cast && Equals(cast);
        }

        public override readonly int GetHashCode()
        {
            return IUUID.GetHashCode(m_high, m_low);
        }

        public override readonly string ToString()
        {
            return IUUID.ToString(m_high, m_low);
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
