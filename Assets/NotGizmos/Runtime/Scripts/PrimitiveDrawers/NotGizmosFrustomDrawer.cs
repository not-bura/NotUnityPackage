using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using States = NotBura.Packages.NotGizmosDrawStates;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Frustom")]
    [Serializable]
    public sealed class NotGizmosFrustomDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private Vector3 m_center   = Vector3.zero;
        [SerializeField] private float m_fov        = 60.0f;
        [SerializeField] private float m_maxRange   = 10.0f;
        [SerializeField] private float m_minRange   = 1.0f;
        [SerializeField] private float m_aspect     = 1.0f;

        public Vector3 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_center;
        }

        public float FOV
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_fov;
        }

        public float MaxRange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_maxRange;
        }

        public float MinRange
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_minRange;
        }

        public float Aspect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_aspect;
        }

        public override void Draw(Context context, States state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(context, _current);

            Gizmos.DrawFrustum(m_center, m_fov, m_maxRange, m_minRange, m_aspect);
        }
    }
}
