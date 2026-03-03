using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Singleton = NotBura.Packages.MasterStringScriptableSingleton;

namespace NotBura.Packages
{
    [FilePath(PATH, FilePathAttribute.Location.ProjectFolder)]
    public sealed class MasterStringScriptableSingleton
        : ScriptableSingleton<Singleton>
    {
        private const string PATH = "Library/NotBura/MasterString.yaml";

        [SerializeField] private List<ITMProMasterString> m_instances;
    }
}
