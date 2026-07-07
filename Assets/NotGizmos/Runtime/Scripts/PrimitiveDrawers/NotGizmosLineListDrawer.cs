using NotBura.Packages;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura
{
    [NotGizmosDrawer("LineList")]
    [Serializable]
    public class NotGizmosLineListDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private Vector3[] m_points = new Vector3[]
        {
            new(0.0f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f),
            new(1.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 0.0f),
        };

        public Vector3[] Points
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_points;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_points = value;
        }

        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode type)
        {
            var _context = m_context;

            {
                Gizmos.color = baseContext.Color * _context.Color;
                Gizmos.matrix = baseContext.Matrix * _context.Matrix;
            }

            Gizmos.DrawLineList(m_points);
        }
    }
}
