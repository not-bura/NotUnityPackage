using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Singleton = NotBura.Packages.MasterStringScriptableSingleton;
using Window = NotBura.Packages.MasterStringEditorWindow;
using Cursor = UnityEngine.UIElements.Cursor;
using System.Linq.Expressions;

namespace NotBura.Packages
{
    using System.Reflection;
    using UnityEngine.UIElements;

    namespace YourNameSpace
    {
        /// <summary>
        /// Settings the cursor via code. Thanks to:
        /// https://discussions.unity.com/t/uielements-style-cursor-in-code/762774/13
        /// <br />
        /// Usage: style.cursor = UnityDefaultCursor.DefaultCursor(UnityDefaultCursor.CursorType.ResizeHorizontal);
        /// </summary>
        public static class UnityDefaultCursor
        {
            public enum CursorType
            {
                Arrow = 0,
                Text = 1,
                ResizeVertical = 2,
                ResizeHorizontal = 3,
                Link = 4,
                SlideArrow = 5,
                ResizeUpRight = 6,
                ResizeUpLeft = 7,
                MoveArrow = 8,
                RotateArrow = 9,
                ScaleArrow = 10,
                ArrowPlus = 11,
                ArrowMinus = 12,
                Pan = 13,
                Orbit = 14,
                Zoom = 15,
                FPS = 16,
                CustomCursor = 17,
                SplitResizeUpDown = 18,
                SplitResizeLeftRight = 19
            }

            private static PropertyInfo _defaultCursorId;

            private static PropertyInfo DefaultCursorId
            {
                get
                {
                    if (_defaultCursorId != null)
                        return _defaultCursorId;

                    _defaultCursorId = typeof(Cursor)
                        .GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance);

                    return _defaultCursorId;
                }
            }

            public static Cursor DefaultCursor(CursorType cursorType)
            {
                var ret = (object)new Cursor();
                DefaultCursorId.SetValue(ret, (int)cursorType);
                return (Cursor)ret;
            }
        }
    }

    public interface IMasterStringEditorView
    {

    }

    public sealed class MasterStringEditorWindow
        : EditorWindow
    {
        public struct Element
        {
            public int Key;
            public string Value;
        }

        private const string PATH = "Window/Text/MasterString";

        [SerializeField] private Singleton m_singleton;

        [MenuItem(PATH)]
        private static void Open()
        {
            var _window = GetWindow<Window>();
            _window.m_singleton = Singleton.instance;
        }

        private static bool SetInt(ref int source)
        {
            var _previous = source;
            var _value = EditorGUILayout.IntField(_previous);
            if (_previous != _value)
            {
                source = _value;
                return true;
            }

            return false;
        }

        private void CreateGUI()
        {
            var v = new TabView();
            var _tabs = new Tab[]
            {
                new("List"),
                new("Container"),
            };
            rootVisualElement.style.cursor = YourNameSpace.UnityDefaultCursor.DefaultCursor(YourNameSpace.UnityDefaultCursor.CursorType.Arrow);
            v.style.cursor =
                YourNameSpace.UnityDefaultCursor.DefaultCursor(YourNameSpace.UnityDefaultCursor.CursorType.Arrow);

            foreach (var _tab in _tabs)
            {
                _tab.style.cursor = YourNameSpace.UnityDefaultCursor.DefaultCursor(YourNameSpace.UnityDefaultCursor.CursorType.Arrow);
                v.Add(_tab);
            }

            rootVisualElement.Add(v);
        }

        private void OnGUI()
        {
            var _singleton = GetSingleton();

            SetInt(ref _singleton.m_showItemCount);

            var _items = _singleton.Elements;

            if (GUILayout.Button("Add"))
            {
                _items.Add(new());
            }

            using (var _scroll = new EditorGUILayout.ScrollViewScope(_singleton.m_scrollPosition))
            {
                _singleton.m_scrollPosition = _scroll.scrollPosition;

                var _offset = _singleton.m_pageIndex * _singleton.m_showItemCount;
                var cnt = (_items.Count - _offset) < _singleton.m_showItemCount
                    ? (_items.Count - _offset)
                    : _singleton.m_showItemCount;

                for (int i = 0; i < cnt; ++i)
                {
                    using var _ = new EditorGUILayout.HorizontalScope();

                    //EditorGUILayout.Foldout(true, GUIContent.none);

                    var _element = _items[_offset + i];

                    GUILayout.Label($"{_element.Key}");
                    var v = EditorGUILayout.TextField(_element.Value);
                    GUILayout.Label($"{v?.Length ?? 0}");

                    _element.Value = v;

                    _items[_offset + i] = _element;
                }
            }

            static bool SetInt(ref int source)
            {
                var _previous = source;
                var _value = EditorGUILayout.IntField(_previous);
                if (_previous != _value)
                {
                    source = _value;
                    return true;
                }

                return false;
            }

            using (var _horizontal = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("-"))
                {
                    if (_singleton.m_pageIncrementCount > 1)
                    {
                        --_singleton.m_pageIncrementCount;
                    }
                }

                SetInt(ref _singleton.m_pageIncrementCount);

                if (GUILayout.Button("+"))
                {
                    if (_singleton.m_pageIncrementCount != int.MaxValue)
                    {
                        ++_singleton.m_pageIncrementCount;
                    }
                }

                if (GUILayout.Button("|<<"))
                {
                    if (_singleton.m_pageIndex != 0)
                    {
                        _singleton.m_pageIndex = 0;
                    }
                }

                void PreviousPageIndex(ref int index, int value)
                {
                    if (index == 0)
                    {
                        return;
                    }

                    var _changed = index - value;
                    index = _changed < 0
                        ? 0
                        : _changed;
                }

                var _count = Mathf.CeilToInt(_items.Count / _singleton.m_showItemCount);

                void NextPageIndex(ref int index, int value)
                {
                    if (index == _count)
                    {
                        return;
                    }

                    var _changed = unchecked(index + value);
                    index = _changed < index
                        ? _count
                        : _changed;
                }

                if (GUILayout.Button("<<"))
                {
                    PreviousPageIndex(ref _singleton.m_pageIndex, _singleton.m_pageIncrementCount);
                }

                if (GUILayout.Button("<"))
                {
                    PreviousPageIndex(ref _singleton.m_pageIndex, 1);
                }

                SetInt(ref _singleton.m_pageIndex);

                if (GUILayout.Button(">"))
                {
                    NextPageIndex(ref _singleton.m_pageIndex, 1);
                }

                if (GUILayout.Button(">>"))
                {
                    NextPageIndex(ref _singleton.m_pageIndex, _singleton.m_pageIncrementCount);
                }

                if (GUILayout.Button(">>|"))
                {
                    if (_singleton.m_pageIndex < _count)
                    {
                        _singleton.m_pageIndex = _count;
                    }
                }
            }
        }

        private Singleton GetSingleton()
        {
            if (m_singleton == null)
            {
                m_singleton = Singleton.instance;
            }

            return m_singleton;
        }
    }
}
