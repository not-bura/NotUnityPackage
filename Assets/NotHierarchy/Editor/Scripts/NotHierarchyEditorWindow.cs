using UnityEditor;
using UnityEngine;
using Singleton = NotBura.Packages.NotHierarchyScriptableSingleton;
using Window = NotBura.Packages.NotHierarchyEditorWindow;

namespace NotBura.Packages
{
    public sealed class NotHierarchyEditorWindow
        : EditorWindow
    {
        private Singleton m_instance;

        [MenuItem("Window/General/NotHierarchy")]
        private static void Open()
        {
            GetWindow<Window>();
        }

        private static void Enable(Singleton instance)
        {
            if (instance.Enabled)
            {
                return;
            }

            instance.Enabled = true;

            NotHierarchyHandler.AddHandler();
        }

        private static void Disable(Singleton instance)
        {
            if (false == instance.Enabled)
            {
                return;
            }

            instance.Enabled = false;

            NotHierarchyHandler.RemoveHandler();
        }

        private void OnEnable()
        {
            m_instance = Singleton.instance;
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Enable"))
            {
                Enable(m_instance);

                m_instance.Save();
            }

            if (GUILayout.Button("Disable"))
            {
                Disable(m_instance);

                m_instance.Save();
            }
        }
    }
}
