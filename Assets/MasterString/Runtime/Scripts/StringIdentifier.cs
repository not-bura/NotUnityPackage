using System;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct StringIdentifier
        : IEquatable<StringIdentifier>
        , IComparable<StringIdentifier>
    {
        [SerializeField] private int m_identifer;

        public StringIdentifier(int identifier)
        {
            m_identifer = identifier;
        }

        public bool IsValid()
        {
            return m_identifer == 0;
        }

        public bool Equals(StringIdentifier other)
        {
            return m_identifer == other.m_identifer;
        }

        public int CompareTo(StringIdentifier other)
        {
            return m_identifer.CompareTo(other.m_identifer);
        }

        public override bool Equals(object obj)
        {
            return obj is StringIdentifier _cast && Equals(_cast);
        }

        public override int GetHashCode()
        {
            return m_identifer;
        }

        public static bool operator ==(StringIdentifier lhs, StringIdentifier rhs)
        {
            return lhs.m_identifer == rhs.m_identifer;
        }

        public static bool operator !=(StringIdentifier lhs, StringIdentifier rhs)
        {
            return lhs.m_identifer != rhs.m_identifer;
        }
    }
}
