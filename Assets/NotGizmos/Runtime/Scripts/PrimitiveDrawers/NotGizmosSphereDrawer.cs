using NotBura.Packages;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura
{
    [NotGizmosDrawer("Sphere")]
    [Serializable]
    public class NotGizmosSphereDrawer
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

        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode type)
        {
            var _context = m_context;

            Gizmos.color = baseContext.Color * _context.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(baseContext, _context);

            switch (type)
            {
                case NotGizmosDrawMode.Default:
                    if (m_isWire)
                    {
                        Gizmos.DrawWireSphere(m_center, Radius);
                    }
                    else
                    {
                        Gizmos.DrawSphere(m_center, Radius);
                    }
                    break;
                case NotGizmosDrawMode.ForcePlane:
                    Gizmos.DrawSphere(m_center, Radius);
                    break;
                case NotGizmosDrawMode.ForceWire:
                    Gizmos.DrawWireSphere(m_center, m_radius);
                    break;
            }
        }
    }
}
