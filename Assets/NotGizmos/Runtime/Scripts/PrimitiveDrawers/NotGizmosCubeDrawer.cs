using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using Types = NotBura.Packages.NotGizmosDrawMode;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Cube")]
    [Serializable]
    public sealed class NotGizmosCubeDrawer
        : NotGizmosDrawer
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

        public override void Draw(Context baseContext, Types type)
        {
            var _thisContext = m_context;

            Gizmos.color = baseContext.Color * _thisContext.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(baseContext, _thisContext);

            switch (type)
            {
                case Types.Default:
                    {
                        if (m_isWire)
                        {
                            Gizmos.DrawWireCube(m_center, m_size);
                        }
                        else
                        {
                            Gizmos.DrawCube(m_center, m_size);
                        }
                        return;
                    }
                case Types.ForcePlane:
                    {
                        Gizmos.DrawCube(m_center, m_size);
                        return;
                    }
                case Types.ForceWire:
                    {
                        Gizmos.DrawWireCube(m_center, m_size);
                        return;
                    }
            }
        }
    }
}
