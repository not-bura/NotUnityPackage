using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NotBura.Packages
{
    public static class HeaderFunctionHandler
    {
        public static Action<object> ResultHandler = DefaultResultHandler;

        private static Texture2D m_iconTexture2D;
        private static Dictionary<Type, HeaderFunctionMenu> m_caches;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            m_iconTexture2D = EditorGUIUtility.FindTexture("PlayButton");

            var _methods = TypeCache.GetMethodsWithAttribute<HeaderFunctionAttribute>();
            m_caches = new();
            var _caches = m_caches;
            foreach (var method in _methods)
            {
                // NOTE: Staticは一旦弾く
                // TODO: Static含め対応したほうが便利なものは対応する
                if (method.IsStatic)
                {
                    continue;
                }

                var _key = method.DeclaringType;
                if (false == _caches.TryGetValue(_key, out var _value))
                {
                    _value = new();
                    _caches.Add(_key, _value);
                }

                _value.Add(method);
            }

            HeaderFunctionEditorHeaderItem.Handler = OnHeaderEditorItem;
        }

        private static bool OnHeaderEditorItem(Rect rectangle, Object[] targetObjects)
        {
            var _type = targetObjects[0].GetType();
            if (m_caches.TryGetValue(_type, out var _menu))
            {
                if (GUI.Button(rectangle, m_iconTexture2D, EditorStyles.iconButton))
                {
                    _menu.Show(targetObjects);
                }

                return true;
            }

            return false;
        }

        private static void DefaultResultHandler(object source)
        {
            Debug.Log(source);
        }
    }
}
