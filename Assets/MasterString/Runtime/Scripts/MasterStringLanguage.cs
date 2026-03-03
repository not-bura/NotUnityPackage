using System;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct MasterStringLanguage
        : IEquatable<MasterStringLanguage>
        , IComparable<MasterStringLanguage>
    {
        [SerializeField] private int m_id;

        public MasterStringLanguage(int id)
        {
            m_id = id;
        }

        public static MasterStringLanguage From(SystemLanguage source)
        {
            return new((int)source);
        }

        public static bool operator ==(MasterStringLanguage lhs, MasterStringLanguage rhs)
        {
            return lhs.m_id == rhs.m_id;
        }

        public static bool operator !=(MasterStringLanguage lhs, MasterStringLanguage rhs)
        {
            return lhs.m_id != rhs.m_id;
        }

        public bool Equals(MasterStringLanguage other)
        {
            return m_id == other.m_id;
        }

        public int CompareTo(MasterStringLanguage other)
        {
            return m_id.CompareTo(other.m_id);
        }

        public override bool Equals(object obj)
        {
            return obj is MasterStringLanguage _cast && Equals(_cast);
        }

        public override int GetHashCode()
        {
            return m_id;
        }
    }
}
