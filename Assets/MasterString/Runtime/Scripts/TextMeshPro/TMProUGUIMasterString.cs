using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace NotBura.Packages
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class TMProUGUIMasterString
        : MaskableGraphic
        , ITMProMasterString
    {
        [SerializeField] private StringIdentifier m_id;

        public StringIdentifier Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_id;
        }

        protected override void Awake()
        {
            MasterStringTrackerBridge.Register(this);
        }

        protected override void OnDestroy()
        {
            MasterStringTrackerBridge.Unregister(this);
        }
    }
}
