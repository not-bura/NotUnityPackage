using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public class MasterStringEditorModel
    {
        [ContextMenuItem("Text", nameof(Initialize))]
        [SerializeField] private List<string> m_texts;

        [SerializeField] private string m_buffer;
        [SerializeField] private List<Element> m_elements;

        [Serializable]
        public struct Element
        {
            public string text;
            public int Start;
            public int Length;
        }

        public void Initialize()
        {
            m_elements = new();

            var (r, d) = Optimize(m_texts);
            m_buffer = r;

            foreach (var e in d)
            {
                m_elements.Add(new Element()
                {
                    text = e.Key,
                    Start = e.Value.Item1,
                    Length = e.Value.Item2,
                });
            }
        }

        private (string, Dictionary<string, (int, int)>) Optimize(IEnumerable<string> input)
        {
            // 1. 重複排除と包含関係の除去（「こんにちは」があれば「こん」を消す）
            var words = input
                .Distinct()
                .OrderByDescending(s => s.Length)
                .ToList();
            var uniqueWords = words
                .Where(w => !words
                    .Any(other =>
                        other != w
                        && other.Contains(w)
                    )
                )
                .ToList();

            // 2. 結合処理（重なりを統合）
            var combined = uniqueWords.Count > 0
                ? uniqueWords[0]
                : "";
            var remaining = uniqueWords
                .Skip(1)
                .ToList();

            while (remaining.Count > 0)
            {
                int bestOverlap = -1;
                int bestIndex = 0;

                for (int i = 0; i < remaining.Count; i++)
                {
                    // combinedの末尾とremaining[i]の先頭の重なりをチェック
                    int overlap = GetMaxOverlap(combined, remaining[i]);
                    if (overlap > bestOverlap)
                    {
                        bestOverlap = overlap;
                        bestIndex = i;
                    }
                }

                // 結合
                var nextWord = remaining[bestIndex];
                combined += nextWord.Substring(bestOverlap);
                remaining.RemoveAt(bestIndex);
            }

            // 3. 元の全ワードに対して、結合後のバッファ内のどこにあるかを記録
            var resultMap = input
                .Distinct()
                .ToDictionary(
                    word => word,
                    word => (combined.IndexOf(word), word.Length)
                );

            return (combined, resultMap);
        }

        // 文字列aの末尾と文字列bの先頭が何文字一致するかを返す
        private int GetMaxOverlap(string a, string b)
        {
            int max = Math.Min(a.Length, b.Length);
            for (int len = max; len > 0; len--)
            {
                if (a.EndsWith(b.Substring(0, len))) return len;
            }
            return 0;
        }
    }
}
