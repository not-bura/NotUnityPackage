using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Singleton = NotBura.Packages.NotProjectBrowserSingleton;

namespace NotBura.Packages
{
    public static class NotProjectBrowser
    {
        [InitializeOnLoadMethod]
        private static void Entry()
        {
            SubscribeProjectWindowItemInstanceOnGUI();
            SubscribeGlobalEventHander<EditorApplication.CallbackFunction>(OnGlobalEventHandler);
            SubscribeUpdate();
        }

        #region project window item instance on gui

        private static void SubscribeProjectWindowItemInstanceOnGUI()
        {
            EditorApplication.projectWindowItemInstanceOnGUI += (id, rect) =>
            {
                if (rect.x < 16.0f)
                {
                    return;
                }

                var _rect = rect;
                _rect.width += _rect.x * 2.0f;
                _rect.x = 0.0f;
                _rect.y += rect.height - 1.0f;
                _rect.height = 1.0f;
                DrawRect(_rect, Color.black);

                if (rect.x == 16.0f)
                {
                    return;
                }

                var _depth = (rect.x - 16.0f) / 14.0f;

                _rect.y = rect.y;
                _rect.width = 2.0f * _depth;
                _rect.height = rect.height;

                var color = Color.HSVToRGB((_depth - 1) * 0.1f % 1.0f, 0.6f, 0.8f);

                DrawRect(_rect, color);
            };

            static void DrawRect(in Rect rect, in Color color)
            {
                DrawTexture(rect, EditorGUIUtility.whiteTexture, color);
            }

            static void DrawTexture(in Rect rect, Texture texture, in Color color)
            {
                var vec4Zero = Vector4.zero;
                GUI.DrawTexture(
                    rect,
                    texture,
                    ScaleMode.StretchToFill,
                    true,
                    0.0f,
                    color,
                    vec4Zero,
                    vec4Zero
                    );
            }
        }

        #endregion project window item instance on gui

        #region global event handler

        private static void SubscribeGlobalEventHander<T>(T callback)
            where T : Delegate
        {
            const string METHOD_NAME = "globalEventHandler";
            const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Static;

            var _handler = typeof(EditorApplication).GetField(METHOD_NAME, BINDING_FLAGS);
            if (_handler is null)
            {
                Debug.LogError($"{METHOD_NAME} is not found on Reflection");
                return;
            }

            var _value = (Delegate)_handler.GetValue(null);
            _value = Delegate.Combine(_value, callback);
            _handler.SetValue(null, _value);
        }

        private static void OnGlobalEventHandler()
        {
            var _current = Event.current;
            if (_current is null)
            {
                return;
            }

            // NOTE: MouseUp以外のイベントは恐らくProjectProwserに吸われて回収できないのでUp
            if (_current.type != EventType.MouseUp)
            {
                return;
            }

            // NOTE: 手前側のサイドボタン
            if (_current.button == 3)
            {
                if (IsActiveProjectBrowser(out var _target))
                {
                    var _singleton = Singleton.instance;
                    var _view = _singleton.Resolve(_target);
                    _view.Undo();

                    _current.Use();
                }

                return;
            }

            // NOTE: 奥側のサイドボタン
            if (_current.button == 4)
            {
                if (IsActiveProjectBrowser(out var _target))
                {
                    var _singleton = Singleton.instance;
                    var _view = _singleton.Resolve(_target);
                    _view.Redo();

                    _current.Use();
                }

                return;
            }
        }

        #endregion global event handler

        private static void SubscribeUpdate()
        {
            EditorApplication.update += Initialize;
        }

        private static void Initialize()
        {
            EditorApplication.update -= Initialize;

            var _targets = ProjectBrowserHandle.GetAllProjectBrowsers();
            foreach (var _target in _targets)
            {
                if (_target.IsInvalid())
                {
                    continue;
                }

                Refresh(_target);
            }

            OnUpdate();

            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            Singleton.instance.Refresh();

            if (IsActiveProjectBrowser(out var _target))
            {
                Refresh(_target);
            }
        }

        private static bool IsActiveProjectBrowser(out ProjectBrowserHandle target)
        {
            var _focused = EditorWindow.focusedWindow;
            if (_focused == null)
            {
                Unsafe.SkipInit(out target);
                return false;
            }

            var _last = ProjectBrowserHandle.LastInteractedProjectBrowser;
            if (_last is null)
            {
                Unsafe.SkipInit(out target);
                return false;
            }

            if (_focused != _last.Value)
            {
                Unsafe.SkipInit(out target);
                return false;
            }

            target = _last.Value;
            return true;
        }

        private static void Refresh(in ProjectBrowserHandle target)
        {
            var _singleton = Singleton.instance;
            _singleton.Upadate(target);
        }
    }
}
