using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility = NotBura.Packages.TMProMasterStringUtility;

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

    public interface ITMProMasterStringSource
    {
        public ReadOnlySpan<char> GetText();
    }

    [Serializable]
    public class TMProMasterStringSource
        : ITMProMasterStringSource
    {
        [SerializeField] private MasterString m_source;
        
        public TMProMasterStringSource(MasterString source)
        {
            m_source = source;
        }

        public ReadOnlySpan<char> GetText()
        {
            return MasterStringAPI.MasterStringProvider.Resolve(m_source.Id);
        }
    }

    [Serializable]
    public class TMProValueStringSource
        : ITMProMasterStringSource
    {
        [SerializeField] private ValueString m_source;

        public TMProValueStringSource(ValueString source)
        {
            m_source = source;
        }

        public ReadOnlySpan<char> GetText()
        {
            return MasterStringAPI.ValueStringProvider.Resolve(m_source.Id);
        }
    }

#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
    public class TMProUGUIMasterString
        : MaskableGraphic
        , ITMProMasterString
    {
        [SerializeReference] private ITMProMasterStringSource m_source;
        [SerializeField] private TMP_FontAsset m_asset;

        public override Texture mainTexture
        {
            get
            {
                if (m_asset == null)
                {
                    return null;
                }

                return m_asset.atlasTexture;
            }
        }

        protected override void Awake()
        {
            MasterStringTrackerBridge.Register(this);
        }

        protected override void OnDestroy()
        {
            MasterStringTrackerBridge.Unregister(this);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (m_asset == null || m_source is null)
            {
                return;
            }

            var _text = m_source.GetText();
            if (_text.Length == 0)
            {
                return;
            }

            var _pos = 0.0f;

            for (int i = 0; i < _text.Length; ++i)
            {
                var _char = _text[i];

                if (false == TMProMasterStringContainer.TryGetCharactor(_char, m_asset, out var _tmp))
                {
                    return;
                }

                var glyph = _tmp.glyph;
                var rect = glyph.glyphRect;
                var metrics = glyph.metrics;

                float scale = 18.0f / m_asset.faceInfo.pointSize;

                // ======== 重要：ベースライン補正 ========
                float baseline = m_asset.faceInfo.ascentLine * scale;

                // グリフの描画位置
                float x0 = metrics.horizontalBearingX * scale + _pos;
                float y0 = baseline - metrics.horizontalBearingY * scale;

                float w = metrics.width * scale;
                float h = metrics.height * scale;

                // ======== Quad の頂点 ========
                Vector3 bl = new(x0, y0 - h);
                Vector3 tl = new(x0, y0);
                Vector3 tr = new(x0 + w, y0);
                Vector3 br = new(x0 + w, y0 - h);

                // ======== UV ========
                float aw = m_asset.atlasWidth;
                float ah = m_asset.atlasHeight;

                Vector2 uv0 = new(rect.x / aw, rect.y / ah);
                Vector2 uv1 = new((rect.x + rect.width) / aw, (rect.y + rect.height) / ah);

                // ======== 頂点追加 ========
                vh.AddVert(bl, color, new Vector4(uv0.x, uv0.y, 0, 0.42f));
                vh.AddVert(tl, color, new Vector4(uv0.x, uv1.y, 0, 0.42f));
                vh.AddVert(tr, color, new Vector4(uv1.x, uv1.y, 0, 0.42f));
                vh.AddVert(br, color, new Vector4(uv1.x, uv0.y, 0, 0.42f));

                vh.AddTriangle(i * 4, i * 4 + 1, i * 4 + 2);
                vh.AddTriangle(i * 4 + 2, i * 4 + 3, i * 4);

                _pos += w;
            }
        }

        public struct TextProcessing
        {
            public enum ProcessType
            {
                Undefined       = 0x0,
                TextCharactor   = 0x1,
                TextMarkup      = 0x2,
            }

            public ProcessType Type;
            public uint Unicode;
            public int Index;
            public int Length;
        }

        [SerializeField] private bool m_isParseControl = true;
        [SerializeField] private bool m_isRichText = true;
        private List<TextProcessing> m_processes;

        public void SetText(ReadOnlySpan<char> source)
        {
            var _writeIndex = 0;
            var _length = source.Length;

            var _results = new List<char>();
            for (int i = 0; i < source.Length; ++i)
            {
                var _target = (uint)source[i];
                if (_target == 0)//'\0'
                {
                    break;
                }

                // NOTE: パース処理
                if (CheckParse(source, _target, ref i, ref _writeIndex))
                {
                    continue;
                }

                // NOTE: タグ処理

                _results.Add(source[i]);
            }

            SetDirtyInternal();
        }

        private bool CheckParse(ReadOnlySpan<char> source, uint target, ref int read, ref int write)
        {
            var _length = source.Length;

            const uint ESCAPE_SEQUENCE = '\\';
            if (target != ESCAPE_SEQUENCE || read >= (_length - 1))
            {
                return false;
            }

            var _value = (uint)source[read + 1];

            switch (_value)
            {
                case 85: // \U00000000 UTF32
                    if (_length > read + 9 &&　Utility.IsValidUTF32(source, read + 2))
                    {
                        var _unicode = Utility.GetUTF32(source, read + 2);
                        m_processes.Add(GetParseProcessing(_unicode, read, 10));
                        read += 9;
                        ++write;
                        return true;
                    }
                    break;
                case 92: // \
                    if (m_isParseControl)
                    {
                        ++read;
                        return false;
                    }
                    break;
                case 110:// \n
                    return Return(10, ref read, ref write);
                case 114:// \r
                    return Return(13, ref read, ref write);
                case 116:// \t
                    return Return( 9, ref read, ref write);
                case 117:// \u0000 UTF-16
                    if (_length > read + 5 && Utility.IsValidUTF16(source, read + 2))
                    {
                        var _unicode = Utility.GetUTF16(source, read + 2);
                        m_processes.Add(GetParseProcessing(_unicode, read, 6));
                        read += 5;
                        ++write;
                        return true;
                    }
                    break;
                case 118:// \v
                    return Return(11, ref read, ref write);
            }

            bool Return(uint unicode, ref int read, ref int write)
            {
                if (false == m_isParseControl)
                {
                    return false;
                }

                m_processes.Add(GetParseProcessing(unicode, read, 1));
                ++read;
                ++write;
                return true;
            }

            TextProcessing GetParseProcessing(uint unicode, int index, int length)
            {
                return new TextProcessing
                {
                    Type = TextProcessing.ProcessType.TextCharactor,
                    Unicode = unicode,
                    Index = index,
                    Length = length,
                };
            }

            return false;
        }

        public void SetText(MasterString source)
        {
            // TODO: Heapから逃がせるものを逃がす
            m_source = new TMProMasterStringSource(source);
            SetText(m_source.GetText());
        }

        public void SetText(ValueString source)
        {
            // TODO: Heapから逃がせるものを逃がす
            m_source = new TMProValueStringSource(source);
            SetText(m_source.GetText());
        }

        private void SetDirtyInternal()
        {
            //{
            //    if (false == IsActive())
            //    {
            //        return;
            //    }

            //    LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            //    m_OnDirtyLayoutCallback.Invoke();

            //    // m_VertsDirty = true;
            //    CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            //    m_OnDirtyVertsCallback?.Invoke();
            //}

            SetVerticesDirty();
            SetLayoutDirty();
        }
    }
}
