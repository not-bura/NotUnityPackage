using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct MasterString
        : IEquatable<MasterString>
        , IComparable<MasterString>
    {
        [SerializeField] private StringIdentifier m_id;

#if UNITY_EDITOR
        public const string EDITOR_ONLY_NAME_ID = nameof(m_id);
#endif

        internal StringIdentifier Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_id;
        }

        public MasterString(StringIdentifier id)
        {
            m_id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
        {
            return m_id.IsValid();
        }

        public bool Equals(MasterString other)
        {
            return m_id.Equals(other.m_id);
        }

        public int CompareTo(MasterString other)
        {
            return m_id.CompareTo(other.m_id);
        }

        public override bool Equals(object obj)
        {
            return obj is MasterString _cast && Equals(_cast);
        }

        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }
    }
}
