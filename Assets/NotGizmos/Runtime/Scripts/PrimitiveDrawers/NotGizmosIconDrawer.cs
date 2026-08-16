using System;
using UnityEngine;
using Context = NotBura.Packages.NotGizmosDrawContext;
using States = NotBura.Packages.NotGizmosDrawStates;

namespace NotBura.Packages
{
    [NotGizmosDrawer("Icon")]
    [Serializable]
    public sealed class NotGizmosIconDrawer
        : BaseNotGizmosDrawer
    {
        [SerializeField] private Vector3 m_position;
        [SerializeField] private string m_name;
        [SerializeField] private bool m_allowScaling = true;
        [SerializeField] private Color m_tint;

        public override void Draw(Context context, States state)
        {
            var _current = m_context;

            Gizmos.color = context.Color * _current.Color;
            Gizmos.matrix = NotGizmosUtility.Matrix(context, _current);

            Gizmos.DrawIcon(m_position, m_name, m_allowScaling, m_tint);
        }
    }
}
