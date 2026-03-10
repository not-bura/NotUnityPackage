using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct AssetIdentifier
    {
        [SerializeField] private uint m_value;

        public uint Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_value;
        }

        public AssetIdentifier(uint value)
        {
            m_value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
        {
            return m_value != 0;
        }

        public override int GetHashCode()
        {
            return (int)m_value;
        }
    }
}
