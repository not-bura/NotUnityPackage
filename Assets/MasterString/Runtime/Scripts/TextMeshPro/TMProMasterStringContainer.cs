using System;
using System.Collections.Generic;
using TMPro;

namespace NotBura.Packages
{
    public static class TMProMasterStringContainer
    {
        private static List<Element> m_elements;
        private static List<byte> m_binary;

        public struct Element
        {
            public int Key;
            public string Value;
        }

        public static int Generate(ReadOnlySpan<char> source)
        {
            return 0;
        }

        public static bool TryGetCharactor(char key, TMP_FontAsset asset, out TMP_Character character)
        {
            return asset.characterLookupTable.TryGetValue(key, out character);
        }
    }

}
