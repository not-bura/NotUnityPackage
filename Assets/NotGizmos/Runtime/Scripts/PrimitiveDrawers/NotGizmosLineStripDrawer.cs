using NotBura.Packages;
using System;
using UnityEngine;

namespace NotBura
{
    [NotGizmosDrawer("LineStrip")]
    [Serializable]
    public class NotGizmosLineStripDrawer
        : NotGizmosDrawer
    {
        [SerializeField] private bool m_isLoop = true;
        [SerializeField] Vector3[] m_points = new Vector3[]
        {
            new(0.0f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f),
            new(1.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 0.0f),
        };

        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode type)
        {
            var _context = m_context;

            {
                Gizmos.color = baseContext.Color * _context.Color;
                Gizmos.matrix = baseContext.Matrix * _context.Matrix;
            }

            Gizmos.DrawLineStrip(m_points, m_isLoop);
        }
    }
}
