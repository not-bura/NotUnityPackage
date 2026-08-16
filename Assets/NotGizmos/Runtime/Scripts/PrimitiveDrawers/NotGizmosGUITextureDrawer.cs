using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using States = NotBura.Packages.NotGizmosDrawStates;

namespace NotBura.Packages
{
    [NotGizmosDrawer("GUITexture")]
    [Serializable]
    public sealed class NotGizmosGUITextureDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private Rect m_rect;
        [SerializeField] private Texture m_texture;
        [SerializeField] private RectOffset m_border;
        [SerializeField] private Material m_material;

        public Rect Rect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_rect;
        }

        public Texture Texture
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_texture;
        }

        public RectOffset Border
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_border;
        }

        public Material Material
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_material;
        }

        public override void Draw(Context context, States state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(context, _current);

            var _border = m_border;
            var _left   = _border.left;
            var _right  = _border.right;
            var _top    = _border.top;
            var _bottom = _border.bottom;

            Gizmos.DrawGUITexture(m_rect, m_texture, _left, _right, _top, _bottom, m_material);
        }
    }
}
