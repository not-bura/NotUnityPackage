using UnityEngine;

namespace NotBura.Packages
{
    /// <summary>
    /// Extensions draw gizmos component.
    /// </summary>
    [AddComponentMenu("NotBura/NotGizmosTrigger")]
    public sealed class NotGizmosTrigger
        : MonoBehaviour
    {
        [SerializeField] private bool m_onSelected;
        [SerializeField] private NotGizmosProperty m_property;

        private void OnDrawGizmos()
        {
            if (m_onSelected)
            {
                return;
            }

            m_property.Draw();
        }

        private void OnDrawGizmosSelected()
        {


            m_property.Draw();
        }
    }
}
