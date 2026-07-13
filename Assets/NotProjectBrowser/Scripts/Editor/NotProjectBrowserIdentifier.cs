using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using FieldType =
#if UNITY_6000_3_OR_NEWER
    UnityEngine.EntityId
#else
    System.Int32
#endif
    ;

namespace NotBura.Packages
{
    [Serializable]
    public struct ProjectBrowserIdentifier
    {
        [SerializeField] private FieldType m_value;
        
        private ProjectBrowserIdentifier(FieldType source)
        {
            m_value = source;
        }

        public string ToAssetPath()
        {
            return AssetDatabase.GetAssetPath(m_value);
        }

        public override bool Equals(object obj)
        {
            return obj is ProjectBrowserIdentifier _cast && EqualsInternal(this, _cast);
        }

        public override int GetHashCode()
        {
            return m_value;
        }

        public static ProjectBrowserIdentifier From(Object source)
        {
            var _value =
#if UNITY_6000_3_OR_NEWER
                source.GetEntityId()
#else
                source.GetInstanceID()
#endif
                ;

            return new(_value);
        }

        public static FieldType[] ToRaws(ProjectBrowserIdentifier[] sources)
        {
            var _results = new FieldType[sources.Length];
            for (int i = 0; i < sources.Length; ++i)
            {
                _results[i] = sources[i].m_value;
            }

            return _results;
        }

        public static bool operator ==(ProjectBrowserIdentifier lhs, ProjectBrowserIdentifier rhs)
        {
            return EqualsInternal(lhs, rhs);
        }

        public static bool operator !=(ProjectBrowserIdentifier lhs, ProjectBrowserIdentifier rhs)
        {
            return NotEqualsInternal(lhs, rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool EqualsInternal(in ProjectBrowserIdentifier lhs, in ProjectBrowserIdentifier rhs)
        {
            return lhs.m_value == rhs.m_value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool NotEqualsInternal(in ProjectBrowserIdentifier lhs, in ProjectBrowserIdentifier rhs)
        {
            return lhs.m_value != rhs.m_value;
        }
    }
}
