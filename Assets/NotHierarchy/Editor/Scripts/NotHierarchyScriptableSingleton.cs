using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Singleton = NotBura.Packages.NotHierarchyScriptableSingleton;

namespace NotBura.Packages
{
    [FilePath(
        "Library/NotBura/NotHierarchy/Singleton.yaml",
        FilePathAttribute.Location.ProjectFolder
    )]
    public class NotHierarchyScriptableSingleton
        : ScriptableSingleton<Singleton>
    {
        [SerializeField] private bool m_enabled = false;

        public bool Enabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_enabled;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_enabled = value;
        }

        public void Save() => instance.Save(true);
    }
}
