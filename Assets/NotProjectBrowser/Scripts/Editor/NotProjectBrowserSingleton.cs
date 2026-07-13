using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Singleton = NotBura.Packages.NotProjectBrowserSingleton;

namespace NotBura.Packages
{
    public sealed class NotProjectBrowserSingleton
        : ScriptableSingleton<Singleton>
    {
        [SerializeField] private List<Relationship> m_relationships = new();
        private List<View> m_views = new();

        public void Refresh()
        {
            var _items = m_views;
            for (int i = 0;  i < _items.Count; ++i)
            {
                var _item = _items[i];
                if (_item.IsInvalid())
                {
                    _item.Dispose();
                    _items.RemoveAt(i);
                    --i;
                }
            }
        }

        public View Resolve(ProjectBrowserHandle source)
        {
            var _relationship = _GetOrCreate();

            var _items = m_views;
            for (int i = 0, loop = _items.Count; i < loop; ++i)
            {
                var _item = _items[i];

                if (_item.Handle == source)
                {
                    return _item;
                }
            }

            var _result = new View(source, _relationship.Value);
            _items.Add(_result);
            return _result;

            Relationship _GetOrCreate()
            {
                var _key = source.GetIdentifier();

                var _items = m_relationships;
                for (int i = 0, loop = _items.Count; i < loop; ++i)
                {
                    var _item = _items[i];
                    if (_key == _item.Id)
                    {
                        return _item;
                    }
                }

                var _result = new Relationship(_key);
                _items.Add(_result);
                return _result;
            }
        }

        public void Upadate(ProjectBrowserHandle source)
        {
            var _result = Resolve(source);
            _result.Update();
        }

        public sealed class View
            : IDisposable
        {
            private readonly ProjectBrowserHandle m_handle;
            private readonly History m_history;

            private bool m_isTwoColumns;
            private long m_lastUpdateTime;
            private string m_lastSearchedText;

            private Button m_undoButton;
            private Button m_redoButton;

            public ProjectBrowserHandle Handle
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => m_handle;
            }

            public View(ProjectBrowserHandle target, History history)
            {
                m_handle = target;
                m_history = history;

                m_isTwoColumns = target.IsTwoColumns();

                m_undoButton = _CreateButton("d_back", 26.0f, m_isTwoColumns, OnClickUndo);
                m_redoButton = _CreateButton("d_forward", 44.0f, m_isTwoColumns, OnClickRedo);

                var _root = target.rootVisualElement;

                _root.Add(m_undoButton);
                _root.Add(m_redoButton);

                _root.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

                m_undoButton.SetEnabled(history.CanUndo);
                m_redoButton.SetEnabled(history.CanRedo);

                static Button _CreateButton(string path, float left, bool visible, Action callback)
                {
                    var _texture = EditorGUIUtility.Load(path) as Texture2D;
                    var _background = Background.FromTexture2D(_texture);

                    return new Button(callback)
                    {
                        visible = visible,
                        iconImage = _background,
                        focusable = false,
                        style =
                        {
                            width = 18.0f,
                            height = 18.0f,
                            position = new(Position.Absolute),
                            left = left,
                            paddingLeft = 0.0f,
                            paddingRight = 0.0f,
                            paddingTop = 0.0f,
                            paddingBottom = 0.0f,
                        },
                    };
                }
            }

            public void Dispose()
            {
                m_undoButton.RemoveFromHierarchy();
                m_redoButton.RemoveFromHierarchy();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsInvalid()
            {
                return m_handle.IsInvalid();
            }

            public void Update()
            {
                var _current = m_handle;

                if (false == UpdateColumns(_current))
                {
                    return;
                }

                UpdateSearchedText(_current);
                UpdateSelectedFolder(_current);
            }

            public void Undo()
            {
                if (m_history.TryUndo(out var _result))
                {
                    Refresh(_result);
                }
            }

            public void Redo()
            {
                var a = m_history.Current;
                if (m_history.TryRedo(out var _result))
                {
                    Refresh(_result);
                }
            }

            private bool UpdateColumns(in ProjectBrowserHandle target)
            {
                var current = target.IsTwoColumns();
                if (m_isTwoColumns != current)
                {
                    m_isTwoColumns = current;
                    m_undoButton.visible = current;
                    m_redoButton.visible = current;
                }

                return current;
            }

            private void UpdateSearchedText(in ProjectBrowserHandle target)
            {
                var _searchedText = target.SearchFieldText;
                var _now = Stopwatch.GetTimestamp();

                if (_searchedText != m_lastSearchedText)
                {
                    m_lastUpdateTime = _now + TimeSpan.FromSeconds(2).Ticks;
                    m_lastSearchedText = _searchedText;
                }

                if (_now < m_lastUpdateTime)
                {
                    return;
                }

                if (string.IsNullOrEmpty(_searchedText))
                {
                    return;
                }

                var _searchViewState = target.GetSearchViewState();
                if (_searchViewState == ProjectBrowserHandle.SearchViewState.NotSearching)
                {
                    return;
                }

                var _current = m_history.Current;

                if (_current is null)
                {
                    var _ids = FoldersToIds(target.LastFolders);

                    var _item = new Record
                    {
                        FolderInstanceIDs = _ids,
                        SearchedText = _searchedText,
                        SearchViewState = _searchViewState,
                    };
                    m_history.Record(_item);
                    return;
                }

                // 検索文字列が最新履歴と変わったら履歴に追加
                if (_searchedText != _current.SearchedText)
                {
                    var _item = new Record
                    {
                        FolderInstanceIDs = _current.FolderInstanceIDs,
                        SearchedText = _searchedText,
                        SearchViewState = _searchViewState,
                    };
                    m_history.Record(_item);
                }

                // 検索範囲が変わっただけなら、最新履歴の検索範囲を更新
                if (_searchViewState != _current.SearchViewState)
                {
                    _current.SearchViewState = _searchViewState;
                }
            }

            private void UpdateSelectedFolder(in ProjectBrowserHandle target)
            {
                var _folders = target.LastFolders;
                if (_folders is null || _folders.Length == 0)
                {
                    return;
                }

                var _ids = FoldersToIds(_folders);

                if (_ids == null || _ids.Length == 0)
                {
                    return;
                }

                _ids = _ids
                    .Where(x => AssetDatabase.IsValidFolder(x.ToAssetPath()))
                    .ToArray();
                var _last = m_history.Current;
                var lastSelectedFolderInstanceIds = _last?.FolderInstanceIDs;
                var isFirstFolderSelected = lastSelectedFolderInstanceIds == null;

                // 初めてフォルダを選択した、もしくは選択フォルダが最新履歴と変わったら履歴に追加
                if (isFirstFolderSelected || !_ids.SequenceEqual(lastSelectedFolderInstanceIds))
                {
                    // 検索範囲が選択フォルダ内なら検索を維持しつつフォルダ選択され、それ以外の検索範囲なら検索はリセットされる
                    var isSearchedSubFolders = _last?.SearchViewState == ProjectBrowserHandle.SearchViewState.SubFolders;
                    var searchedText = isSearchedSubFolders ? _last.SearchedText : null;
                    var searchViewState = isSearchedSubFolders
                        ? ProjectBrowserHandle.SearchViewState.SubFolders
                        : ProjectBrowserHandle.SearchViewState.NotSearching;

                    // 履歴に追加する
                    var current = new Record
                    {
                        FolderInstanceIDs = _ids,
                        SearchedText = searchedText,
                        SearchViewState = searchViewState,
                    };

                    m_history.Record(current);
                    Refresh(current);
                }
            }

            private void Refresh(Record source)
            {
                if (source.FolderInstanceIDs != null && source.FolderInstanceIDs.Length != 0)
                {
                    m_handle.SetFolderSelection(source.FolderInstanceIDs, false);
                }

                if (false == string.IsNullOrWhiteSpace(source.SearchedText))
                {
                    var _filter = SearchFilterHandle.CreateSearchFilterFromString(source.SearchedText);
                    _filter.Folders = source.FolderInstanceIDs
                        .Select(x => x.ToAssetPath())
                        .ToArray();
                    m_handle.SetSearch(_filter);
                }

                if (source.SearchViewState != ProjectBrowserHandle.SearchViewState.NotSearching)
                {
                    m_handle.SetSearchViewState(source.SearchViewState);
                }

                m_undoButton.SetEnabled(m_history.CanUndo);
                m_redoButton.SetEnabled(m_history.CanRedo);
            }

            private void OnClickUndo()
            {
                Undo();
            }

            private void OnClickRedo()
            {
                Redo();
            }

            private void OnSizeChanged(GeometryChangedEvent e)
            {
                var _isTwoColumns = m_handle.IsTwoColumns();
                if (false == _isTwoColumns)
                {
                    return;
                }
                
                if (e.newRect.width > 532.0f)
                {
                    if (false == m_undoButton.visible)
                    {
                        m_undoButton.visible = true;
                        m_redoButton.visible = true;
                    }
                    return;
                }

                if (m_undoButton.visible)
                {
                    m_undoButton.visible = false;
                    m_redoButton.visible = false;
                }
            }

            private ProjectBrowserIdentifier[] FoldersToIds(string[] folders)
            {
                var _results = new List<ProjectBrowserIdentifier>();

                for (int i = 0; i < folders.Length; ++i)
                {
                    var _folder = folders[i];
                    var _cast = AssetDatabase.LoadAssetAtPath<Object>(_folder);

                    if (_cast == null)
                    {
                        continue;
                    }

                    var _item = ProjectBrowserIdentifier.From(_cast);
                    _results.Add(_item);
                }

                return _results.ToArray();
            }
        }

        [Serializable]
        public sealed class Relationship
        {
            [SerializeField] private ProjectBrowserIdentifier m_id;
            [SerializeField] private History m_value;

            public ProjectBrowserIdentifier Id
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => m_id;
            }

            public History Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => m_value;
            }

            public Relationship(ProjectBrowserIdentifier id)
            {
                m_id = id;
                m_value = new();
            }
        }

        [Serializable]
        public sealed class History
        {
            [SerializeField] private List<Record> m_records;
            [SerializeField] private int m_index;

#nullable enable
            public Record? Current
#nullable disable
            {
                get => m_records.Count == 0
                    ? null
                    : m_records[m_index];
            }

            public bool CanUndo
            {
                get
                {
                    return m_records.Count > 0
                        && m_index > 0;
                }
            }

            public bool CanRedo
            {
                get
                {
                    return m_records.Count > 0
                        && m_index < m_records.Count - 1;
                }
            }

            public History()
            {
                m_records = new();
                m_index = 0;
            }

            public void Record(Record source)
            {
                var _records = m_records;
                var _count = _records.Count;

                if (_count > 0)
                {
                    var _last = _count - 1;
                    var _index = m_index;
                    if (_last != _index)
                    {
                        _records.RemoveRange(_index, _last - _index);
                    }
                }

                m_index = _records.Count;
                _records.Add(source);
            }

            public bool TryUndo(out Record result)
            {
                if (m_records.Count == 0)
                {
                    Unsafe.SkipInit(out result);
                    return false;
                }

                if (m_index == 0)
                {
                    Unsafe.SkipInit(out result);
                    return false;
                }

                result = m_records[--m_index];
                return true;
            }

            public bool TryRedo(out Record result)
            {
                if (m_records.Count == 0)
                {
                    Unsafe.SkipInit(out result);
                    return false;
                }

                if (m_index >= m_records.Count - 1)
                {
                    Unsafe.SkipInit(out result);
                    return false;
                }

                result = m_records[++m_index];
                return true;
            }
        }

        [Serializable]
        public sealed class Record
        {
            public ProjectBrowserIdentifier[] FolderInstanceIDs;
            public string SearchedText;
            public ProjectBrowserHandle.SearchViewState SearchViewState;
        }
    }
}
