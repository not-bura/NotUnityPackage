using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    public enum NotGizmosDrawMode
    {
        Default,
        ForcePlane,
        ForceWire,
    }

    [Serializable]
    public sealed class NotGizmosProperty
    {
        [SerializeField] private bool m_enabled = true;
        [SerializeField] private NotGizmosDrawContext m_context = new();
        [SerializeField] private NotGizmosDrawMode m_drawMode = NotGizmosDrawMode.Default;
        [SerializeReference] private List<NotGizmosDrawer> m_drawers = new();

        public const string EDITOR_ONLY_NAME_ENABLED = nameof(m_enabled);
        public const string EDITOR_ONLY_NAME_CONTEXT = nameof(m_context);
        public const string EDITOR_ONLY_NAME_DRAW_STATE = nameof(m_drawMode);
        public const string EDITOR_ONLY_NAME_ELEMENTS = nameof(m_drawers);

        public bool Enabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_enabled;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_enabled = value;
        }

        public NotGizmosDrawContext Context
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_context;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_context = value;
        }

        public NotGizmosDrawMode DrawMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_drawMode;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_drawMode = value;
        }

        public List<NotGizmosDrawer> Drawers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_drawers;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_drawers = value;
        }

        public void Draw()
        {
            if (false == m_enabled)
            {
                return;
            }

            var _prevColor = Gizmos.color;
            var _prevMatrix = Gizmos.matrix;

            var _context = m_context;
            var _drawMode = m_drawMode;

            var _drawers = m_drawers;

            try
            {
                foreach (var drawer in _drawers)
                {
                    if (drawer is null || false == drawer.Enabled)
                    {
                        continue;
                    }

                    drawer.Draw(_context, _drawMode);
                }
            }
            finally
            {
                Gizmos.color = _prevColor;
                Gizmos.matrix = _prevMatrix;
            }
        }
    }
}
