using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using States = NotBura.Packages.NotGizmosDrawStates;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Sphere")]
    [Serializable]
    public sealed class NotGizmosSphereDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private bool m_isWire = false;
        [SerializeField] private Vector3 m_center = Vector3.zero;
        [SerializeField] private float m_radius = 1.0f;

        public bool IsWire
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_isWire;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_isWire = value;
        }

        public Vector3 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_center;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_center = value;
        }

        public float Radius
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_radius;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_radius = value;
        }

        public override void Draw(Context context, States state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(context, _current);

            if (state is States.ForceWire || state is States.Default && m_isWire)
            {
                Gizmos.DrawWireSphere(m_center, m_radius);
                return;
            }

            Gizmos.DrawSphere(m_center, m_radius);
        }
    }
}
