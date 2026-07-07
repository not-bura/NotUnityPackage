using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct StringIdentifier
        : IEquatable<StringIdentifier>
        , IComparable<StringIdentifier>
    {
        [SerializeField] private int m_value;

#if UNITY_EDITOR
        public const string EDITOR_ONLY_NAME_VALUE = nameof(m_value);

        public int EditorOnlyValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_value;
        }
#endif

        public StringIdentifier(int value)
        {
            m_value = value;
        }

        public bool IsValid()
        {
            return m_value <= 0;
        }

        public bool Equals(StringIdentifier other)
        {
            return m_value == other.m_value;
        }

        public int CompareTo(StringIdentifier other)
        {
            return m_value.CompareTo(other.m_value);
        }

        public override bool Equals(object obj)
        {
            return obj is StringIdentifier _cast && Equals(_cast);
        }

        public override int GetHashCode()
        {
            return m_value;
        }

        public static bool operator ==(StringIdentifier lhs, StringIdentifier rhs)
        {
            return lhs.m_value == rhs.m_value;
        }

        public static bool operator !=(StringIdentifier lhs, StringIdentifier rhs)
        {
            return lhs.m_value != rhs.m_value;
        }
    }
}
