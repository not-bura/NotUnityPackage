using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    [CustomPropertyDrawer(typeof(NotGizmosDrawContext))]
    public sealed class NotGizmosDrawerContextPropertyDrawer : PropertyDrawer
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

        public override void OnGUI(Rect pos_, SerializedProperty prop_, GUIContent label_)
        {
            var _singleLine = EditorGUIUtility.singleLineHeight;
            var _lineHeight = _singleLine + EditorGUIUtility.standardVerticalSpacing;
            var _width = pos_.width;

            pos_.height = _singleLine;

            {
                prop_.isExpanded = EditorGUI.Foldout(pos_, prop_.isExpanded, label_);
                pos_.y += _lineHeight;
            }

            if (prop_.isExpanded)
            {
                EditorGUI.indentLevel++;

                var _colorProp = m_colorProp ??= prop_.FindPropertyRelative("m_color");
                var _transformProp = m_transformProp ??= prop_.FindPropertyRelative("m_transform");
                var _posProp = m_positionProp ??= prop_.FindPropertyRelative("m_position");
                var _rotProp = m_rotationProp ??= prop_.FindPropertyRelative("m_rotation");
                var _scaleProp = m_scaleProp ??= prop_.FindPropertyRelative("m_scale");

                EditorGUI.PropertyField(pos_, _colorProp, COLOR_CONTENT);
                pos_.y += _lineHeight;

                EditorGUI.PropertyField(pos_, _transformProp, TRANSFORM_CONTENT);
                pos_.y += _lineHeight;

                EditorGUI.PropertyField(pos_, _posProp, POSITION_CONTENT);
                pos_.y += _lineHeight;

                EditorGUI.PropertyField(pos_, _rotProp, ROTATION_CONTENT);
                pos_.y += _lineHeight;

                EditorGUI.PropertyField(pos_, _scaleProp, SCALE_CONTENT);
                pos_.y += _lineHeight;

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
