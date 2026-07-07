using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    [CustomPropertyDrawer(typeof(NotGizmosDrawContext))]
    public sealed class NotGizmosDrawerContextPropertyDrawer
        : PropertyDrawer
    {
        private static readonly GUIContent COLOR_CONTENT        = new("Color");
        private static readonly GUIContent TRANSFORM_CONTENT    = new("Transform");
        private static readonly GUIContent POSITION_CONTENT     = new("Position");
        private static readonly GUIContent ROTATION_CONTENT     = new("Rotation");
        private static readonly GUIContent SCALE_CONTENT        = new("Scale");

        private SerializedProperty m_colorProp;
        private SerializedProperty m_transformProp;
        private SerializedProperty m_positionProp;
        private SerializedProperty m_rotationProp;
        private SerializedProperty m_scaleProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _singleLine = EditorGUIUtility.singleLineHeight;
            var _lineHeight = _singleLine + EditorGUIUtility.standardVerticalSpacing;
            var _width = position.width;

            position.height = _singleLine;

            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
                position.y += _lineHeight;
            }

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var _colorProp = m_colorProp ??= property.FindPropertyRelative("m_color");
                var _transformProp = m_transformProp ??= property.FindPropertyRelative("m_transform");
                var _posProp = m_positionProp ??= property.FindPropertyRelative("m_position");
                var _rotProp = m_rotationProp ??= property.FindPropertyRelative("m_rotation");
                var _scaleProp = m_scaleProp ??= property.FindPropertyRelative("m_scale");

                EditorGUI.PropertyField(position, _colorProp, COLOR_CONTENT);
                position.y += _lineHeight;

                EditorGUI.PropertyField(position, _transformProp, TRANSFORM_CONTENT);
                position.y += _lineHeight;

                EditorGUI.PropertyField(position, _posProp, POSITION_CONTENT);
                position.y += _lineHeight;

                EditorGUI.PropertyField(position, _rotProp, ROTATION_CONTENT);
                position.y += _lineHeight;

                EditorGUI.PropertyField(position, _scaleProp, SCALE_CONTENT);
                position.y += _lineHeight;

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty prop_, GUIContent label_)
        {
            var _lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var _height = _lineHeight;

            if (prop_.isExpanded)
            {
                _height += _lineHeight * 5;
            }

            return _height;
        }
    }
}
