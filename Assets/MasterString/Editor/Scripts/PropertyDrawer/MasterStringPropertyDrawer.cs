using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    public static class MasterStringGUI
    {
        public static (bool IsChanged, StringIdentifier Value) StringIdentifierField(
            Rect position
            , GUIContent label
            , SerializedProperty property
        )
        {
            var _valueProperty = property
                .FindPropertyRelative(StringIdentifier.EDITOR_ONLY_NAME_VALUE);

            var _preview = _valueProperty.intValue;
            var _value = EditorGUI.IntField(position, label, _preview);

            var _isChanged = _value != _preview;

            if (_isChanged)
            {
                _valueProperty.intValue = _value;
            }

            return (_isChanged, new(_value));
        }
    }

    [CustomPropertyDrawer(typeof(MasterString))]
    public class MasterStringPropertyDrawer
        : PropertyDrawer
    {
        private StringIdentifier m_cacheId;
        private string m_cacheString;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _content = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            var _id = property.FindPropertyRelative(MasterString.EDITOR_ONLY_NAME_ID);

            position.height = EditorGUIUtility.singleLineHeight;

            var (_isChanged, _value) = MasterStringGUI.StringIdentifierField(position, _content, _id);

            if (_isChanged)
            {
                m_cacheId = _value;

                if (_value.IsValid())
                {
                    if (m_cacheId.IsValid())
                    {
                        m_cacheId = default;
                        m_cacheString = string.Empty;
                    }
                }
                else
                {
                    if (_value != m_cacheId)
                    {
                        var stringProvider = MasterStringAPI.MasterStringProvider;
                        if (stringProvider != null)
                        {
                            var _text = stringProvider.Resolve(_value);
                            if (m_cacheString != _text)
                            {
                                m_cacheString = _text.ToString();
                            }
                        }
                    }
                }
            }

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            var _temp = EditorGUIUtility.TrTempContent(m_cacheString);
            EditorGUI.LabelField(position, label);

            EditorGUI.EndProperty();

            if (EditorGUI.EndChangeCheck())
            {
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2.0f
                + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
