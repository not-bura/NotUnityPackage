using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    public static class HeaderFunctionDrawerFactory
    {
        private static Dictionary<Type, Type> m_drawers;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            m_drawers = new();
            var _drawers = m_drawers;
            var _types = TypeCache.GetTypesWithAttribute<CustomHeaderFunctionDrawerAttirbute>();
            foreach (var type in _types)
            {
                if (false == type.IsSubclassOf(typeof(HeaderFunctionDrawer)))
                {
                    Debug.LogError("");
                }

                var _attribute = type.GetCustomAttribute<CustomHeaderFunctionDrawerAttirbute>();
                if (_drawers.ContainsKey(_attribute.Type))
                {
                    Debug.LogError("");
                    continue;
                }

                _drawers.Add(_attribute.Type, type);
            }
        }

        public static HeaderFunctionDrawer CreateInstance(Type type)
        {
            if (m_drawers.TryGetValue(type, out var _source))
            {
                var _instance = Activator.CreateInstance(_source);
                if (_instance != null)
                {
                    return _instance as HeaderFunctionDrawer;
                }
            }

            _source = typeof(HeaderFunctionDrawer<>).MakeGenericType(type);
            var _default = Activator.CreateInstance(_source);
            return _default as HeaderFunctionDrawer;
        }
    }
}
