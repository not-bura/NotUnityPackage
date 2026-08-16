using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Line")]
    [Serializable]
    public sealed class NotGizmosLineDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private Vector3 m_from = Vector3.zero;
        [SerializeField] private Vector3 m_to = new(1.0f, 0.0f, 0.0f);

        public Vector3 From
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_from;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_from = value;
        }

        public Vector3 To
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_to;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_to = value;
        }

        public override void Draw(NotGizmosDrawContext context, NotGizmosDrawStates states)
        {
            // TODO: 不用なContextが含まれるので全体で適切に扱える方法を考える
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = context.Matrix * _current.Matrix;

            Gizmos.DrawLine(m_from, m_to);
        }
    }
}
