using System;
using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public abstract class HeaderFunctionDrawer
    {
        [SerializeField] protected string m_name;

        public HeaderFunctionDrawer()
        {
        }

        public virtual void Initialize(string name, object source)
        {
            m_name = name;
        }

        public virtual void OnGUI(SerializedProperty serializedProperty)
        {
            EditorGUILayout.LabelField(m_name);
        }

        public virtual object GetValue()
        {
            return null;
        }
    }
}
