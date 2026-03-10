using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct AssetHandle
    {
        [SerializeField] private AssetIdentifier m_id;

        public AssetIdentifier Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_id;
        }

        public AssetHandle(AssetIdentifier id)
        {
            m_id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
        {
            return m_id.IsValid();
        }

        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }
    }

    [Serializable]
    public struct AssetHandle<T>
    {
        [SerializeField] private AssetIdentifier m_id;
        [SerializeField] private T m_value;

        public AssetIdentifier Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_id;
        }

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_value;
        }

        public AssetHandle(AssetIdentifier id, T value)
        {
            m_id = id;
            m_value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
        {
            return m_id.IsValid();
        }

        public override int GetHashCode()
        {
            return m_id.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator AssetHandle(AssetHandle<T> self)
        {
            return new(self.m_id);
        }
    }
}
