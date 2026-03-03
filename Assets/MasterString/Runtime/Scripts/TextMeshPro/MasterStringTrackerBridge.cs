using System.Diagnostics;

namespace NotBura.Packages
{
    public static class MasterStringTrackerBridge
    {
        private const string EDITOR_ONLY = "UNITY_EDITOR";

        [Conditional(EDITOR_ONLY)]
        public static void Register(ITMProMasterString target)
        {
            MasterStringTracker.Register(target);
        }

        [Conditional(EDITOR_ONLY)]
        public static void Unregister(ITMProMasterString target)
        {
            MasterStringTracker.Unregister(target);
        }
    }
}
