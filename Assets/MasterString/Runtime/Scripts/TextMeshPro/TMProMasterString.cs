using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    public class TMProMasterString
        : MonoBehaviour
        , ITMProMasterString
    {
        [SerializeField] private StringIdentifier m_id;

        public StringIdentifier Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_id;
        }

        private void Awake()
        {
            MasterStringTrackerBridge.Register(this);
        }

        private void OnDestroy()
        {
            MasterStringTrackerBridge.Unregister(this);
        }
    }
}
