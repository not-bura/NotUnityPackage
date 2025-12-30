using System;
using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public class HeaderFunctionDrawer<T>
        : HeaderFunctionDrawer
    {
        [SerializeField] private T m_value;

        public override void Initialize(string name, object source)
        {
            m_value = (T)source;
            base.Initialize(name, source);
        }

        public override void OnGUI(SerializedProperty serializedProperty)
        {
            var _valueProperty = serializedProperty.FindPropertyRelative(nameof(m_value));
            var _content = EditorGUIUtility.TempContent(m_name);
            EditorGUILayout.PropertyField(_valueProperty, _content);
        }

        public override object GetValue()
        {
            return m_value;
        }
    }
}
