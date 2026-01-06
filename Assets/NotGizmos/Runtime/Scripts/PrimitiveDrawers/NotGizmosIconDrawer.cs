using System;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using Types = NotBura.Packages.NotGizmosDrawMode;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Icon")]
    [Serializable]
    public sealed class NotGizmosIconDrawer
        : NotGizmosDrawer
    {
        [SerializeField] private Vector3 m_position;
        [SerializeField] private string m_name;
        [SerializeField] private bool m_allowScaling = true;
        [SerializeField] private Color m_tint;

        public override void Draw(Context baseContext, Types type)
        {
            Gizmos.matrix = NotGizmosUtility.Matrix(baseContext, m_context);

            Gizmos.color = baseContext.Color * m_context.Color;

            Gizmos.DrawIcon(m_position, m_name, m_allowScaling, m_tint);
        }
    }
}
