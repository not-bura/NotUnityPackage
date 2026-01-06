using NotBura.Packages;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura
{
    [NotGizmosDrawer("Ray")]
    [Serializable]
    public class NotGizmosRayDrawer
        : NotGizmosDrawer
    {
        [SerializeField] private Vector3 m_from = Vector3.zero;
        [SerializeField] private Vector3 m_direction = new(1.0f, 0.0f, 0.0f);

        public Vector3 From
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_from;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_from = value;
        }

        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode type)
        {
            var _context = m_context;

            {
                Gizmos.color = baseContext.Color * _context.Color;
                Gizmos.matrix = baseContext.Matrix * _context.Matrix;
            }

            // NOTE: 内部的にはDrawLineと等価
            Gizmos.DrawRay(m_from, m_direction);
        }
    }
}
