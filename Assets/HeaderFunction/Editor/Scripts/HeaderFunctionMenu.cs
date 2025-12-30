using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NotBura.Packages
{
    public class HeaderFunctionMenu
    {
        public static Action<object> ResultHandler = context =>
        {
            Debug.Log(context);
        };
        private List<HeaderFunctionMenuData> m_elements;

        private class UserData
        {
            public HeaderFunctionMenuData Data;
            public Object[] Targets;
        }

        public HeaderFunctionMenu()
        {
            m_elements = new();
        }

        public void Add(MethodInfo methodInfo)
        {
            var _attirbute = methodInfo.GetCustomAttribute<HeaderFunctionAttribute>();
            m_elements.Add(new(methodInfo, _attirbute));
        }

        public void Show(Object[] targets)
        {
            var _menu = new GenericMenu();

            var _elements = m_elements;
            foreach (var element in _elements)
            {
                _menu.AddItem(new GUIContent(element.Name), false, Invoke, new UserData
                {
                    Data = element,
                    Targets = targets
                });
            }

            _menu.ShowAsContext();
        }

        private void Invoke(object userData)
        {
            var _cast = (UserData)userData;
            var _element = _cast.Data;
            var _targets = _cast.Targets;

            var _arguments = _element.GetFixedArguments(out var _openWindow);

            // NOTE: ModalWindow等ではObjectField等がAllowSceneできないので苦肉の実装
            if (_openWindow)
            {
                HeaderFunctionEditorWindow.Open(_element.Parameters, _arguments, results =>
                {
                    _element.Invoke(_targets, results);
                });
                return;
            }
            
            _element.Invoke(_targets, _arguments);
        }
    }
}
