using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Ray")]
    [Serializable]
    public sealed class NotGizmosRayDrawer
        : BaseNotGizmosDrawer
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

        public Vector3 Direction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_direction;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_direction = value;
        }

        public override void Draw(NotGizmosDrawContext context, NotGizmosDrawStates state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = context.Matrix * _current.Matrix;

            // NOTE: 内部的にはDrawLineと等価
            Gizmos.DrawRay(m_from, m_direction);
        }
    }
}
