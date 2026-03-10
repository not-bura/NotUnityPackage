using System;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public struct Pointer
    {
        public int Index;
        public uint Version;
    }

    public class Table
    {
        private struct Element
        {
            public int Index;
            public uint Version;
        }

        private Element[] Pointers;
        private int[] FreeIndices;

        public void New()
        {

        }

        public void Delete()
        {

        }
    }
}
