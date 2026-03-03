using System.Collections.Generic;

namespace NotBura.Packages
{
    public static class MasterStringTracker
    {
        private static List<ITMProMasterString> s_instances = new();

        public static List<ITMProMasterString> Instances => s_instances;

        public static void Register(ITMProMasterString target)
        {
            if (s_instances.Contains(target))
            {
                return;
            }

            s_instances.Add(target);
        }

        public static void Unregister(ITMProMasterString target)
        {
            if (false == s_instances.Contains(target))
            {
                return;
            }

            s_instances.Remove(target);
        }
    }
}
