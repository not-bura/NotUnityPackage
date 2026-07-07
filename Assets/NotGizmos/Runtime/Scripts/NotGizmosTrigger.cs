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
        [SerializeField] private NotGizmosProperty p;

        private void OnDrawGizmos()
        {

        }

        private void OnDrawGizmosSelected()
        {

        }
    }
}
