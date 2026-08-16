using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using States = NotBura.Packages.NotGizmosDrawStates;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Mesh")]
    [Serializable]
    public sealed class NotGizmosMeshDrawer
        : BaseNotGizmosDrawer
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

        public override void Draw(Context context, States state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(context, _current);

            if (state is States.ForceWire || state is States.Default && m_isWire)
            {
                Gizmos.DrawWireMesh(m_mesh, m_subMeshIndex, m_postion, m_rotation, m_scale);
                return;
            }

            Gizmos.DrawMesh(m_mesh, m_subMeshIndex, m_postion, m_rotation, m_scale);
        }
    }
}
