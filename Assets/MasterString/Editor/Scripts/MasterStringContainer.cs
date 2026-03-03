using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NotBura.Packages
{
    [CreateAssetMenu(menuName = "MasterString")]
    public class MasterStringContainer : ScriptableObject
    {
        public MasterStringModel Model;

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

        [ContextMenu("Model")]
        public void SetModel()
        {
            var _table = new List<MasterStringModel.Element>()
            {
                new()
                {
                    Language = MasterStringLanguage.From(SystemLanguage.Japanese),
                    Elements = new()
                    {
                        new()
                        {
                            Id = new(0),
                            Name = "こんにちは",
                        },
                    },
                },

                new()
                {
                    Language = MasterStringLanguage.From(SystemLanguage.English),
                    Elements = new()
                    {
                        new()
                        {
                            Id = new(0),
                            Name = "Hello",
                        },
                    },
                },
            };

            var _state = new MasterStringModel()
            {
                Encoding = Encoding.UTF8,
                Language = MasterStringLanguage.From(Application.systemLanguage),
                Table = _table,
            };

            Model = _state;
        }

        [ContextMenu("A")]
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
            // 1. 包含関係の徹底排除
            // 長い順にソートして、既存の大きな文に含まれる小さな文を消す
            var sorted = input.Distinct().OrderByDescending(s => s.Length).ToList();
            var mainNodes = new List<string>();
            foreach (var s in sorted)
            {
                if (!mainNodes.Any(m => m.Contains(s)))
                    mainNodes.Add(s);
            }

            // 2. 重なり結合 (Greedy with Overlap)
            // 数千件ある場合は、全ペアチェックを効率化するため
            // 「重なりが大きいペア」から優先的に結合する
            while (mainNodes.Count > 1)
            {
                int bestOverlap = -1;
                int bestI = -1, bestJ = -1;

                // ※本来はここでAho-Corasick等を使うのが理想だが、
                // 簡略化しつつ「直近100件」など範囲を絞って探査するだけでも効果大
                // ここでは「全探索」の構造を示すが、件数に応じて制限をかける
                for (int i = 0; i < Math.Min(mainNodes.Count, 200); i++)
                {
                    for (int j = 0; j < mainNodes.Count; j++)
                    {
                        if (i == j) continue;
                        int ov = GetMaxOverlap(mainNodes[i], mainNodes[j]);
                        if (ov > bestOverlap)
                        {
                            bestOverlap = ov;
                            bestI = i; bestJ = j;
                        }
                    }
                }

                if (bestOverlap > 0)
                {
                    var combined = mainNodes[bestI] + mainNodes[bestJ].Substring(bestOverlap);
                    var valI = mainNodes[bestI];
                    var valJ = mainNodes[bestJ];
                    mainNodes.Remove(valI);
                    mainNodes.Remove(valJ);
                    mainNodes.Insert(0, combined); // 結合したものを先頭に置いて再利用しやすくする
                }
                else break;
            }

            // 3. 最終バッファの構築
            var buffer = string.Join("", mainNodes);

            // 4. 循環最適化 (最後と最初が重なるなら削る)
            int cyclicOverlap = GetMaxOverlap(buffer, buffer);
            if (cyclicOverlap > 0 && cyclicOverlap < buffer.Length)
            {
                // 循環参照を許容する場合、末尾の重複分をカットできる
                // buffer = buffer.Substring(0, buffer.Length - cyclicOverlap);
            }

            // 5. マッピング作成 (すべての元単語がどこにあるか)
            var resultMap = input.Distinct().ToDictionary(
                word => word,
                word => {
                    int index = buffer.IndexOf(word);
                    // もし循環最適化したなら、ここで剰余計算 index % buffer.Length を考慮する
                    return (index, word.Length);
                }
            );

            return (buffer, resultMap);
        }

        private int GetMaxOverlap(string a, string b)
        {
            int max = Math.Min(a.Length, b.Length) - 1;
            for (int len = max; len > 0; len--)
            {
                // Spanを使うと数千件でも高速
                if (a.AsSpan().EndsWith(b.AsSpan(0, len))) return len;
            }
            return 0;
        }
    }
}
