using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [NotGizmosDrawer("LineList")]
    [Serializable]
    public sealed class NotGizmosLineListDrawer
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

        public override void Draw(NotGizmosDrawContext context, NotGizmosDrawStates state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = context.Matrix * _current.Matrix;

            Gizmos.DrawLineList(m_points);
        }
    }
}
