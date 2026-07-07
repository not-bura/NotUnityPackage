using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;

namespace NotBura.Packages
{
    [Serializable]
    public sealed class FileMasterStringProvider
        : IMasterStrinProvider<StringIdentifier>
    {
        public ref struct Handle
        {
            private NativeArray<char> m_value;

            public Handle(NativeArray<char> value)
            {
                m_value = value;
            }

            public ReadOnlySpan<char> GetValue()
            {
                return m_value.AsReadOnlySpan();
            }

            public void Dispose()
            {
                m_value.Dispose();
                m_value = default;
            }
        }

        private string m_path;
        private Encoding m_encoding;

        private NativeArray<Edge> m_edges;

        private struct Edge
        {
            public StringIdentifier Id;
            public long Offset;
            public int Length;
        }

        private struct EdgeComparer
            : IComparer<Edge>
        {
            public int Compare(Edge x, Edge y)
            {
                var _result = x.Id.CompareTo(y.Id);

                if (0 != _result)
                {
                    return _result;
                }

                if (x.Offset == y.Offset && x.Length == y.Length)
                {
                    return 0;
                }

                throw new InvalidOperationException($"Duplicate entry for {x.Id}");
            }
        }

        public FileMasterStringProvider()
        {
        }

        private bool TryGetValue(StringIdentifier id, out Edge result)
        {
            var _key = new Edge
            {
                Id = id,
                Offset = -1,
                Length = -1,
            };

            var _index = m_edges.BinarySearch(_key, new EdgeComparer());

            if (-1 == _index)
            {
                result = default;
                return false;
            }

            result = m_edges[_index];
            return true;
        }

        public ReadOnlySpan<char> Resolve(StringIdentifier id)
        {
            if (TryGetValue(id, out var _edge))
            {
                using var _reader = new DirectFileReader(m_path);
                using var _result = _reader.GetNativeCharArray(m_encoding, _edge.Offset, _edge.Length, Allocator.Temp);

                return _result.AsReadOnlySpan();
            }

            return null;
        }

        public void Dispose()
        {
        }
    }
}
