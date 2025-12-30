using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Window = NotBura.Packages.HeaderFunctionEditorWindow;

namespace NotBura.Packages
{
    public sealed class HeaderFunctionEditorWindow
        : EditorWindow
    {
        [SerializeField] private HeaderFunctionAsset m_asset;

        private SerializedObject m_serializedObject;
        private Action<object[]> m_onClose;

        private static object GetValue(Type type, int index, object[] arguments)
        {
            if (arguments != null
                && arguments[index] != null
                && arguments.Length > index
                && type.IsInstanceOfType(arguments[index])
            )
            {
                return arguments[index];
            }

            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.IsSubclassOf(typeof(Object)))
            {
                return null;
            }

            return Activator.CreateInstance(type);
        }

        public static void Open(ParameterInfo[] parameters, object[] arguments, Action<object[]> onClose)
        {
            var _drawers = new HeaderFunctionDrawer[parameters.Length];
            for (int i = 0; i < _drawers.Length; ++i)
            {
                var _parameter = parameters[i];

                _drawers[i] = HeaderFunctionDrawerFactory.CreateInstance(_parameter.ParameterType);

                var _type = _parameter.ParameterType;
                var _name = _parameter.Name;
                var _argument = GetValue(_type, i, arguments);

                _drawers[i].Initialize(_name, _argument);
            }

            var _asset = ScriptableObject.CreateInstance<HeaderFunctionAsset>();
            _asset.Initialize(_drawers);

            var _window = GetWindow<Window>();
            _window.m_asset = _asset;
            _window.m_serializedObject = new(_asset);
            _window.m_onClose = onClose;
        }

        private void OnGUI()
        {
            m_serializedObject.UpdateIfRequiredOrScript();

            var _property = m_serializedObject.FindProperty("m_drawers");
            if (_property != null)
            {
                var _drawers =  m_asset.Drawers;
                for (int i = 0, len = _property.arraySize; i < len; ++i)
                {
                    var _current = _property.GetArrayElementAtIndex(i);
                    _drawers[i].OnGUI(_current);
                }
            }

            m_serializedObject.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            var _drawers = m_asset.Drawers;
            var _results = new object[_drawers.Length];
            for (int i = 0; i < _results.Length; ++i)
            {
                _results[i] = _drawers[i].GetValue();
            }

            m_onClose?.Invoke(_results);

            DestroyImmediate(m_asset);
            m_asset = null;
            m_serializedObject = null;
        }
    }
}
