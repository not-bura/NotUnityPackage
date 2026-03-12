using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct ValueString
        : IEquatable<ValueString>
        , IComparable<ValueString>
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

        public ValueString(StringIdentifier id)
        {
            m_id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
        {
            return m_id.IsValid();
        }

        public bool Equals(ValueString other)
        {
            return m_id.Equals(other.m_id);
        }

        public int CompareTo(ValueString other)
        {
            return m_id.CompareTo(other.m_id);
        }
    }
}
