using NotBura.Packages;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura
{
    [NotGizmosDrawer("Mesh")]
    [Serializable]
    public class NotGizmosMeshDrawer
        : NotGizmosDrawer
    {
        [SerializeField] private bool m_isWire = false;
        [SerializeField] private Vector3 m_postion = Vector3.zero;
        [SerializeField] private Quaternion m_rotation = Quaternion.identity;
        [SerializeField] private Vector3 m_scale = Vector3.one;
        [SerializeField] private Mesh m_mesh = null;
        [SerializeField] private int m_subMeshIndex = -1;

        public bool IsWire
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_isWire;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_isWire = value;
        }

        public Vector3 Positon
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_postion;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_postion = value;
        }

        public Quaternion Rotation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_rotation;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_rotation = value;
        }

        public Vector3 Scale
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_scale;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_scale = value;
        }

        public Mesh Mesh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_mesh;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_mesh = value;
        }

        public int SubMeshIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_subMeshIndex;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_subMeshIndex = value;
        }

        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode type)
        {
            var _context = m_context;

            Gizmos.color = baseContext.Color * _context.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(baseContext, _context);

            switch (type)
            {
                case NotGizmosDrawMode.Default:
                    {
                        if (m_isWire)
                        {
                            Gizmos.DrawWireMesh(m_mesh, m_subMeshIndex, m_postion, m_rotation, m_scale);
                        }
                        else
                        {
                            Gizmos.DrawMesh(m_mesh, m_subMeshIndex, m_postion, m_rotation, m_scale);
                        }
                    }
                    break;
                case NotGizmosDrawMode.ForcePlane:
                    {
                        Gizmos.DrawMesh(m_mesh, m_subMeshIndex, m_postion, m_rotation, m_scale);
                    }
                    break;
                case NotGizmosDrawMode.ForceWire:
                    {
                        Gizmos.DrawWireMesh(m_mesh, m_subMeshIndex, m_postion, m_rotation, m_scale);
                    }
                    break;
            }
        }
    }
}
