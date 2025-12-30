using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NotBura.Packages
{
    public static class HeaderFunctionEditorHeaderItem
    {
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
                var _key = method.DeclaringType;
                if (false == _caches.TryGetValue(_key, out var _value))
                {
                    _value = new();
                    _caches.Add(_key, _value);
                }

                _value.Add(method);
            }
        }

        [EditorHeaderItem(typeof(Object))]
        private static bool OnEditorHeaderItem(Rect rectangle, Object[] targetObjets)
        {
            var _type = targetObjets[0].GetType();
            if (m_caches.TryGetValue(_type, out var _value))
            {
                if (GUI.Button(rectangle, m_iconTexture2D, EditorStyles.iconButton))
                {
                    _value.Show(targetObjets);
                }

                return true;
            }

            return false;
        }
    }
}
