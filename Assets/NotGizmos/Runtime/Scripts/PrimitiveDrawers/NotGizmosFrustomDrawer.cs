using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using Types = NotBura.Packages.NotGizmosDrawMode;

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

        public override void Draw(Context baseContext, Types type)
        {
            var _thisContext = m_context;

            Gizmos.color = baseContext.Color * _thisContext.Color;

            Gizmos.matrix = NotGizmosUtility.Matrix(baseContext, _thisContext);

            Gizmos.DrawFrustum(m_center, m_fov, m_maxRange, m_minRange, m_aspect);
        }
    }
}
