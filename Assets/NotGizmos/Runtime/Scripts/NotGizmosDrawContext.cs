using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public sealed class NotGizmosDrawContext
    {
        [SerializeField] private Color m_color;
        [SerializeField] private Transform m_transform;
        [SerializeField] private Vector3 m_position;
        [SerializeField] private Quaternion m_rotation;
        [SerializeField] private Vector3 m_scale;

        #region property

        public Color Color
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_color;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_color = value;
        }

        public Transform Transform
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_transform;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_transform = value;
        }

        public Vector3 Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_position;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_position = value;
        }

        public Quaternion Rotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_rotation;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_rotation = value;
        }

        public Vector3 Scale
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_scale;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_scale = value;
        }

        public Matrix4x4 Matrix
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Matrix4x4.TRS(in m_position, in m_rotation, in m_scale);
        }

        #endregion property

        private readonly static Color Default = Color.green;

        public NotGizmosDrawContext()
        {
            m_color = Default;
            m_transform = null;
            m_position = Vector3.zero;
            m_rotation = Quaternion.identity;
            m_scale = Vector3.one;
        }
    }
}
