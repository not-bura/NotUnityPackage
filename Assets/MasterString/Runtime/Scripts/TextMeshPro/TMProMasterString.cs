using System;
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
        [SerializeReference] private ITMProMasterStringSource m_source;

        private void Awake()
        {
            MasterStringTrackerBridge.Register(this);
        }

        private void OnDestroy()
        {
            MasterStringTrackerBridge.Unregister(this);
        }

        public void SetText(ReadOnlySpan<char> source)
        {
        }

        public void SetText(MasterString source)
        {
        }

        public void SetText(ValueString source)
        {
        }
    }
}
