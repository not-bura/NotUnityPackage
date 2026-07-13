using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine.UIElements;
using Identifier =
#if UNITY_6000_3_OR_NEWER
    UnityEngine.EntityId
#else
    System.Int32
#endif
    ;

namespace NotBura.Packages
{
    public struct ProjectBrowserHandle
        : IEquatable<ProjectBrowserHandle>
        , IEquatable<EditorWindow>
    {
        public enum SearchViewState
        {
            NotSearching,
            AllAssets,
            InAssetsOnly,
            InPackagesOnly,
            SubFolders
        }

        public static readonly Type WrapperType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");

        private static readonly Type s_projectBrowserListType = typeof(List<>).MakeGenericType(WrapperType);
        private static readonly Type s_searchViewStateType = WrapperType.GetNestedType("SearchViewState");

        private static readonly FieldInfo s_listInternalArray = s_projectBrowserListType
            .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly StaticROField<EditorWindow> s_lastInteractedProjectBrowser = WrapperType
            .GetPublicStaticField<EditorWindow>("s_LastInteractedProjectBrowser");

        private static readonly ROField<EditorWindow, string[]> s_lastFoldersField = WrapperType
            .GetNonPublicInstanceROField<EditorWindow, string[]>("m_LastFolders");
        private static readonly ROField<EditorWindow, string> s_searchFieldTextField = WrapperType
            .GetNonPublicInstanceROField<EditorWindow, string>("m_SearchFieldText");

        private static readonly InstanceVoidMethod<EditorWindow, Identifier[], bool> s_setFolderSelectionMethod = WrapperType
            .GetNonPublicInstanceVoidMethod<EditorWindow, Identifier[], bool>("SetFolderSelection");
        private static readonly InstanceVoidMethod<EditorWindow, object> s_setSearchMethod = WrapperType
            .GetPublicInstanceVoidMethod<EditorWindow, object>("SetSearch", SearchFilterHandle.WrapperType);

        // NOTE: 一度しか呼ばれないのでExpressionで組み立てない
        private static readonly MethodInfo s_getAllProjectBrowsersMethod = WrapperType
            .GetMethod("GetAllProjectBrowsers", BindingFlags.Public | BindingFlags.Static);

        private static readonly InstanceMethod<EditorWindow, bool> s_isTwoColumns = WrapperType
            .GetNonPublicInstanceMethod<EditorWindow, bool>("IsTwoColumns");
        private static readonly InstanceMethod<EditorWindow, SearchViewState> s_getSearchViewStateMethod = WrapperType
            .GetNonPublicInstanceMethod<EditorWindow, SearchViewState>("GetSearchViewState", s_searchViewStateType);
        private static readonly InstanceVoidMethod<EditorWindow, SearchViewState> s_setSearchViewStateMethod = WrapperType
            .GetNonPublicInstanceVoidMethod<EditorWindow, SearchViewState>("SetSearchViewState", s_searchViewStateType);

        private EditorWindow m_instance;

        #region property

        public static ProjectBrowserHandle? LastInteractedProjectBrowser
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var _result = s_lastInteractedProjectBrowser.Get();

                // NOTE: EditorWindow継承クラスなので演算子でヌルチェック
                if (_result == null)
                {
                    return null;
                }

                return new(_result);
            }
        }

        public VisualElement rootVisualElement
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_instance.rootVisualElement;
        }

        public string[] LastFolders
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => s_lastFoldersField.Get(m_instance);
        }

        public string SearchFieldText
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => s_searchFieldTextField.Get(m_instance);
        }

        #endregion property

        private ProjectBrowserHandle(EditorWindow instance)
        {
            m_instance = instance;
        }

        #region general

        public bool Equals(ProjectBrowserHandle other)
        {
            return EqualsInternal(this, other);
        }

        public bool Equals(EditorWindow other)
        {
            return EqualsInternal(this, other);
        }

        public override bool Equals(object obj)
        {
            return obj is ProjectBrowserHandle _cast &&  EqualsInternal(this, _cast);
        }

        public override int GetHashCode()
        {
            return m_instance.GetHashCode();
        }

        private static bool EqualsInternal(in ProjectBrowserHandle lhs, in ProjectBrowserHandle rhs)
        {
            return lhs.m_instance == rhs.m_instance;
        }

        private static bool NotEqualsInternal(in ProjectBrowserHandle lhs, in ProjectBrowserHandle rhs)
        {
            return lhs.m_instance != rhs.m_instance;
        }

        private static bool EqualsInternal(in ProjectBrowserHandle lhs, EditorWindow rhs)
        {
            return lhs.m_instance == rhs;
        }

        private static bool NotEqualsInternal(in ProjectBrowserHandle lhs, EditorWindow rhs)
        {
            return lhs.m_instance != rhs;
        }

        public static bool operator ==(ProjectBrowserHandle lhs, ProjectBrowserHandle rhs)
        {
            return EqualsInternal(lhs, rhs);
        }

        public static bool operator !=(ProjectBrowserHandle lhs, ProjectBrowserHandle rhs)
        {
            return NotEqualsInternal(lhs, rhs);
        }

        public static bool operator ==(ProjectBrowserHandle lhs, EditorWindow rhs)
        {
            return EqualsInternal(lhs, rhs);
        }

        public static bool operator !=(ProjectBrowserHandle lhs, EditorWindow rhs)
        {
            return NotEqualsInternal(lhs, rhs);
        }

        public static  bool operator ==(EditorWindow lhs, ProjectBrowserHandle rhs)
        {
            return EqualsInternal(rhs, lhs);
        }

        public static bool operator !=(EditorWindow lhs, ProjectBrowserHandle rhs)
        {
            return NotEqualsInternal(rhs, lhs);
        }

        #endregion general

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ProjectBrowserIdentifier GetIdentifier()
        {
            return ProjectBrowserIdentifier.From(m_instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInvalid()
        {
            // NOTE: EditorWindowなのでオーバーライドされている==比較
            return m_instance == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SearchViewState GetSearchViewState()
        {
            return s_getSearchViewStateMethod.Invoke(m_instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSearchViewState(SearchViewState state)
        {
            s_setSearchViewStateMethod.Invoke(m_instance, state);
        }

        public void SetFolderSelection(ProjectBrowserIdentifier[] selectedInstanceIDs, bool revealSelectionAndFrameLastSelected)
        {
            var _values = ProjectBrowserIdentifier.ToRaws(selectedInstanceIDs);
            s_setFolderSelectionMethod.Invoke(m_instance, _values, revealSelectionAndFrameLastSelected);
        }

        public void SetSearch(SearchFilterHandle searchFilter)
        {
            s_setSearchMethod.Invoke(m_instance, searchFilter.Instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTwoColumns()
        {
            return s_isTwoColumns.Invoke(m_instance);
        }

        public static ReadOnlySpan<ProjectBrowserHandle> GetAllProjectBrowsers()
        {
            var _result = s_getAllProjectBrowsersMethod.Invoke(null, null);
            var _array = (object[])s_listInternalArray.GetValue(_result);

            if (_array is null || _array.Length == 0)
            {
                return ReadOnlySpan<ProjectBrowserHandle>.Empty;
            }

            var _cast = new ProjectBrowserHandle[_array.Length];
            var _span = _cast.AsSpan();

            for (int i = 0; i < _span.Length; ++i)
            {
                _span[i] = new ProjectBrowserHandle(_array[i] as EditorWindow);
            }

            return _cast;
        }
    }
}
