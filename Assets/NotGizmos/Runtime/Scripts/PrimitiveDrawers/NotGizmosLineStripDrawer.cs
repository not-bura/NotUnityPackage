using System;
using UnityEngine;

namespace NotBura.Packages
{
    [NotGizmosDrawer("LineStrip")]
    [Serializable]
    public sealed class NotGizmosLineStripDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private bool m_isLoop = true;
        [SerializeField] Vector3[] m_points = new Vector3[]
        {
            new(0.0f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f),
            new(1.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 0.0f),
        };

        public override void Draw(NotGizmosDrawContext context, NotGizmosDrawStates state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = context.Matrix * _current.Matrix;

            Gizmos.DrawLineStrip(m_points, m_isLoop);
        }
    }
}
