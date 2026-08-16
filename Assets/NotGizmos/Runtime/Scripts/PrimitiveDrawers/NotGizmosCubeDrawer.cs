using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using States = NotBura.Packages.NotGizmosDrawStates;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Cube")]
    [Serializable]
    public sealed class NotGizmosCubeDrawer
        : BaseNotGizmosDrawer
        , INotGizmosWire
    {
        [SerializeField] private bool m_isWire      = false;
        [SerializeField] private Vector3 m_center   = Vector3.zero;
        [SerializeField] private Vector3 m_size     = Vector3.one;

        public bool IsWire
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_isWire;
        }

        public Vector3 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_center;
        }

        public Vector3 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_size;
        }

        public override void Draw(Context context, States state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(context, _current);

            if (state is States.ForceWire || state is States.Default && m_isWire)
            {
                Gizmos.DrawWireCube(m_center, m_size);
                return;
            }

            Gizmos.DrawCube(m_center, m_size);
        }
    }
}
