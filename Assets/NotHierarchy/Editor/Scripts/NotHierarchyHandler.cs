using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Singleton = NotBura.Packages.NotHierarchyScriptableSingleton;

namespace NotBura.Packages
{
    public static class NotHierarchyHandler
    {
        private static EditorApplication.HierarchyWindowItemCallback s_cache = OnHierarchyGUI;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            var instance = Singleton.instance;

            if (instance == null || false == instance.Enabled)
            {
                return;
            }

            EditorApplication.hierarchyWindowItemOnGUI += s_cache;
            EditorApplication.RepaintHierarchyWindow();
        }

        public static void AddHandler()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= s_cache;
            EditorApplication.hierarchyWindowItemOnGUI += s_cache;
            EditorApplication.RepaintHierarchyWindow();
        }

        public static void RemoveHandler()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= s_cache;
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void OnHierarchyGUI(int instanceId, Rect selectionRect)
        {
            if (EditorUtility.EntityIdToObject(instanceId) is not GameObject go)
            {
                return;
            }

            HierarchyGUI(go, instanceId, selectionRect);
        }

        private static void HierarchyGUI(GameObject go, int instanceId, in Rect selectionRect)
        {
            // NOTE: 大抵常に何度も呼ばれるので負荷を抑える対策を入れる
            var _event = Event.current;
            if (_event.type == EventType.Repaint)
            {
                CheckMissing(go, selectionRect);
                UnderLine(go, selectionRect);
                Depth(go, selectionRect);
                ComponentIcons(go, selectionRect);
            }

            ActiveToggle(go, instanceId, selectionRect);
        }

        private static void CheckMissing(GameObject go, in Rect selectionRect)
        {
            var _missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (_missingCount == 0)
            {
                return;
            }

            var _rect = new Rect(
                0.0f,
                selectionRect.y,
                selectionRect.x + selectionRect.width + 16.0f,
                selectionRect.height
            );

            var _color = Color.yellow;
            _color.a -= 0.8f;

            DrawRect(_rect, _color);
        }

        private static void UnderLine(GameObject go, in Rect selectionRect)
        {
            const float SIZE = 1.0f;
            var rect = new Rect(
                0.0f,
                selectionRect.y + selectionRect.height - SIZE,
                selectionRect.x + selectionRect.width + 16.0f,
                SIZE
            );

            var _color = Color.black;

            DrawRect(rect, _color);
        }

        private static void Depth(GameObject go, in Rect selectionRect)
        {
            const float SIZE = 16.0f;
            var _depth = GetHierarchyDepth(go.transform);

            if (_depth == 0)
            {
                return;
            }

            var _rect = new Rect(
                52.0f,
                selectionRect.y,
                2.0f * _depth,
                SIZE
            );

            var color = Color.HSVToRGB((_depth - 1) * 0.1f % 1.0f, 0.6f, 0.8f);

            DrawRect(_rect, color);
        }

        private static void ComponentIcons(GameObject go, in Rect selectionRect)
        {
            using (ListPool<Component>.Get(out var _components))
            {
                go.GetComponents(typeof(Component), _components);

                if (_components.Count == 0)
                {
                    return;
                }

                // NOTE: 横幅が一定より小さすぎる時はスキップ
                if (selectionRect.width < 100.0f)
                {
                    return;
                }

                // NOTE: 横幅が一定よりそれなりに小さい時はスキップ
                if (selectionRect.width < 200.0f)
                {
                    DrawComponentIconsMini(_components, selectionRect);
                    return;
                }

                // NOTE: 通常描画
                DrawComponentIcons(_components, selectionRect);
            }
        }

        private static void ActiveToggle(GameObject go, int instanceId, in Rect selectionRect)
        {
            // NOTE: Overrideのデザインに極力被らぬように1ズラす
            var rect = new Rect(
                32.0f + 1.0f,
                selectionRect.y,
                16.0f,
                16.0f
            );

            var _active = GUI.Toggle(rect, go.activeSelf, string.Empty, EditorStyles.radioButton);
            if (go.activeSelf != _active)
            {
                var selectObjects = Selection.objects;
                if (selectObjects.Length == 0 || selectObjects[0] is not GameObject)
                {
                    SetActive(go, _active);
                    return;
                }

                if (TrySetActiveSelections(go, instanceId, selectObjects, _active))
                {
                    return;
                }

                SetActive(go, _active);
            }
        }

        private static int GetHierarchyDepth(Transform transform)
        {
            int depth = 0;
            while (transform.parent != null)
            {
                depth++;
                transform = transform.parent;
            }
            return depth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDefaultComponent(Component component)
        {
            return component is Transform
#if ENABLE_PARTICLE_SYSTEM
                or ParticleSystemRenderer
#endif
                ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawRect(in Rect rect, in Color color)
        {
            DrawTexture(rect, EditorGUIUtility.whiteTexture, color);
        }

        private static void DrawTexture(in Rect rect, Texture texture, in Color color)
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

        private static void DrawComponentIconsMini(List<Component> components, in Rect selectionRect)
        {
            var _color = Color.white;

            const float SIZE = 16.0f;
            const float TEXT_WIDTH = 20.0f;
            var _rect = new Rect(
                selectionRect.xMax - SIZE,
                selectionRect.y,
                SIZE,
                SIZE
            );

            var _count = 0;
            Texture _lastTexture = null;

#if ENABLE_NOT_EXTENSIONS
            // NOTE: 本当に超極小誤差レベルだがSpan処理の中でもforのTop2BottomよりReverseのほうが早い気がした
            var _span = components.AsSpan();
            _span.Reverse();

            for (int i = 0; i < _span.Length; ++i)
            {
                var _component = _span[i];
#else
            for (int i = components.Count - 1; i >= 0; --i)
            {
                var _component = components[i];
#endif

                if (IsDefaultComponent(_component))
                {
                    continue;
                }

                var _texture = AssetPreview.GetMiniThumbnail(_component);
                if (_texture == null)
                {
                    continue;
                }

                _lastTexture = _texture;
                _count++;
            }

            if (_count == 0)
            {
                return;
            }

            if (_count > 1)
            {
                _rect.x -= SIZE;
            }

            DrawTexture(_rect, _lastTexture, _color);

            if (_count > 1)
            {
                _rect.x += SIZE;
                _rect.width = TEXT_WIDTH;

                var text = _count > 9
                    ? $"+N"
                    : $"+{_count - 1}";

                GUI.Label(_rect, text);
            }
        }

        private static void DrawComponentIcons(List<Component> components, in Rect selectionRect)
        {
            const float SIZE = 16.0f;
            var _rect = new Rect(
                selectionRect.xMax - SIZE,
                selectionRect.y,
                SIZE,
                SIZE
            );

            var _enableColor = Color.white;
            var _disableColor = _enableColor;
            _disableColor.a -= 0.3f;

#if ENABLE_NOT_EXTENSIONS
            // NOTE: 本当に超極小誤差レベルだがSpan処理の中でもforのTop2BottomよりReverseのほうが早い気がした
            var _span = components.AsSpan();
            _span.Reverse();

            for (int i = 0; i < _span.Length; ++i)
            {
                var _component = _span[i];
#else
            for (int i = components.Count - 1; i >= 0; --i)
            {
                var _component = components[i];
#endif

                if (IsDefaultComponent(_component))
                {
                    continue;
                }

                var _texture = AssetPreview.GetMiniThumbnail(_component);
                if (_texture == null)
                {
                    continue;
                }

                var _active = EditorUtility.GetObjectEnabled(_component);

                DrawTexture(_rect, _texture, _active == 0
                    ? _disableColor
                    : _enableColor
                );
                _rect.x -= _rect.width;
            }
        }

        private static void SetActive(GameObject go, bool active)
        {
            Undo.RecordObject(go, $"{(active ? "Activate" : "Deactivate")} GameObject '{go.name}'");
            go.SetActive(active);
            EditorUtility.SetDirty(go);
        }

        private static bool TrySetActiveSelections(GameObject go, int instanceId, Object[] selections, bool active)
        {
            foreach (var select in selections)
            {
                if (select.GetInstanceID() == instanceId)
                {
                    Undo.RecordObjects(selections, $"{(active ? "Activate" : "Deactivate")} GameObjects '{go.name} and {selections.Length - 1} '");
                    foreach (var obj in selections)
                    {
                        (obj as GameObject).SetActive(active);
                        EditorUtility.SetDirty(obj);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
