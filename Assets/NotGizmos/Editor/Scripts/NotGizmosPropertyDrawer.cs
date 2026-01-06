using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NotBura.Packages
{
    [CustomPropertyDrawer(typeof(NotGizmosProperty))]
    public sealed class NotGizmosPropertyDrawer
        : PropertyDrawer
    {
        private static Color s_bgColor = new(0.2f, 0.4f, 0.3f, 1.0f);
        private static readonly GUIContent s_enabledGUIContent = new("Enabled");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var _serial = property.serializedObject;
            _serial.UpdateIfRequiredOrScript();

            var _singleLine = EditorGUIUtility.singleLineHeight;
            var _lineHeight = _singleLine + EditorGUIUtility.standardVerticalSpacing;
            var _width = position.width;
            position.height = _singleLine;

            var _isArrayElement = _serial.FindProperty(fieldInfo.Name).isArray;

            {// BG
                var _rect = new Rect(0.0f, 0.0f, _width + 22.0f, _singleLine);
                if (_isArrayElement)
                {
                    _rect.x -= 13.0f;
                    _rect.width += 13.0f;
                    _rect.height += 4.0f;
                }
                EditorGUI.DrawRect(_rect, s_bgColor);
            }

            {// Foldout
                var _rect = new Rect(
                    position.x,
                    _isArrayElement
                        ? position.y
                        : position.y - 2.0f,
                    position.width,
                    position.height
                );

                var _preview = property.isExpanded;
                var _value = EditorGUI.Foldout(_rect, _preview, label);
                if (_value != _preview)
                {
                    property.isExpanded = _value;
                }
            }

            {// Enabled
                var _rect = new Rect(
                    _width - 48.0f,
                    _isArrayElement
                        ? position.y
                        : position.y - 2.0f,
                    position.width,
                    position.height
                );

                EditorGUI.PrefixLabel(_rect, s_enabledGUIContent);
                _rect.x += 52;
                var _prop = property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_ENABLED);
                _prop.boolValue = EditorGUI.Toggle(_rect, _prop.boolValue);

                if (_isArrayElement)
                {
                    position.y += 4;
                }
                position.y += _lineHeight;
            }

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                {
                    var _prop = property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_CONTEXT);
                    EditorGUI.PropertyField(position, _prop);
                    position.y += GetPropertyHeight(_prop);
                }

                {
                    var _prop = property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_DRAW_STATE);
                    _prop.enumValueIndex = EditorGUI.Popup(position, "DrawMode", _prop.enumValueIndex, Enum.GetNames(typeof(NotGizmosDrawMode)));
                    position.y += _lineHeight;
                }

                {
                    var _prop = property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_ELEMENTS);
                    EditorGUI.PropertyField(position, _prop);
                    position.y += GetPropertyHeight(_prop);
                }

                {
                    if (GUI.Button(position, "Add Drawer"))
                    {
                        var _dropdown = new NotGizmosDrawerAdvancedDropdown(property);
                        _dropdown.Show(position);
                    }
                    position.y += _lineHeight;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private static float GetPropertyHeight(SerializedProperty prop_)
        {
            return EditorGUI.GetPropertyHeight(prop_, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var _singlelineHeight = EditorGUIUtility.singleLineHeight;
            var _spacing = EditorGUIUtility.standardVerticalSpacing;
            var _height = _singlelineHeight;

            if (property.isExpanded)
            {
                if (property.serializedObject.FindProperty(fieldInfo.Name).isArray)
                {
                    _height += 4.0f;
                }

                _height += GetPropertyHeight(property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_CONTEXT));

                _height += _spacing;
                _height += _singlelineHeight; // enum

                _height += GetPropertyHeight(property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_ELEMENTS));

                _height += _spacing;
                _height += _singlelineHeight; // button
            }

            return _height;
        }

        public class NotGizmosDrawerAdvancedDropdown : AdvancedDropdown
        {
            private Dictionary<string, Type> m_drawersDictionary = new();
            private SerializedProperty m_serializedProperty;

            public NotGizmosDrawerAdvancedDropdown(SerializedProperty property)
                : base(new AdvancedDropdownState())
            {
                minimumSize = new(200, 200);
                m_serializedProperty = property;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                var _root = new AdvancedDropdownItem("Drawers");
                var _drawers = TypeCache
                    .GetTypesDerivedFrom<NotGizmosDrawer>()
                    .Where(x =>
                        false == x.IsAbstract
                        && false == x.IsInterface
                        && false == x.IsGenericType
                    );

                foreach (var drawer in _drawers)
                {
                    var _attribute = drawer.GetCustomAttribute<NotGizmosDrawerAttribute>();

                    var _name = _attribute == null
                        ? drawer.Name
                        : _attribute.Name;
                    var _item = new AdvancedDropdownItem(_name);
                    m_drawersDictionary.TryAdd(_name, drawer);
                    _root.AddChild(_item);
                }

                return _root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                var _type = m_drawersDictionary[item.name];

                var _property = m_serializedProperty;
                var _serializedObject = _property.serializedObject;

                _serializedObject.UpdateIfRequiredOrScript();

                var _drawers = _property.FindPropertyRelative(NotGizmosProperty.EDITOR_ONLY_NAME_ELEMENTS);
                var _index = _drawers.arraySize;
                _drawers.InsertArrayElementAtIndex(_index);

                var _drawerProperty = _drawers.GetArrayElementAtIndex(_index);
                var _instance = Activator.CreateInstance(_type);
                _drawerProperty.boxedValue = _instance;

                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
