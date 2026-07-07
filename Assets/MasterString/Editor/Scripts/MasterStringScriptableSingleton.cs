using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Singleton = NotBura.Packages.MasterStringScriptableSingleton;

namespace NotBura.Packages
{
    [Serializable]
    public class MasterStringEditorModelA
    {
        public ListModel List;
        public ContainerModel Container;

        [Serializable]
        public class ListModel
        {
            public int PageIndex = 0;
            public int PageItemLimit = 10;
            public int PageIncrementCount = 1;
            public Vector2 ScrollPosition;

            public List<Element> Elements = new();
        }

        [Serializable]
        public class ContainerModel
        {
            public int ByteSize;
            public int CharCount;
            public string Page;
            public int Count;
            public int PageIndex;
        }

        public int m_showItemCount = 10;
        public int m_pageIndex = 0;
        public int m_pageIncrementCount = 1;
        public Vector2 m_scrollPosition;
        public List<Element> Elements = new();

        [Serializable]
        public struct Element
        {
            public bool Foldout;
            public int Key;
            public string Value;
        }

        [Serializable]
        public struct Span
        {
            public int Key;
            public int Start;
            public int Length;
        }
    }

    [FilePath(SINGLETON_PATH, FilePathAttribute.Location.ProjectFolder)]
    public sealed class MasterStringScriptableSingleton
        : ScriptableSingleton<Singleton>
    {
        private const string DIRECTORY_PATH = "Library/NotBura/MasterString";
        private const string SINGLETON_PATH = DIRECTORY_PATH + "/Singleton.yaml";
        private const string EDITOR_PATH    = DIRECTORY_PATH + "/Model.txt";
        private const string EDITOR_TEMPORARY_PATH = DIRECTORY_PATH + "/Model.temp";

        public int m_showItemCount = 10;
        public int m_pageIncrementCount = 1;
        public int m_pageIndex = 0;
        public Vector2 m_scrollPosition;
        public List<Element> Elements = new();

        [Serializable]
        public struct Element
        {
            public bool Foldout;
            public int Key;
            public string Value;
        }

        protected override void Save(bool saveAsText)
        {

            base.Save(saveAsText);
        }
    }
}
