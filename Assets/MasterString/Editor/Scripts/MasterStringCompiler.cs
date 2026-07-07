using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnsafeOptimize;

namespace NotBura.Packages
{
    public interface IMasterStringCompiler
    {
        public void Execute(IEnumerable<StringSource> source);
    }

    public struct StringSource
    {
        public StringIdentifier Id;
        public string Value;

        public StringSource(StringIdentifier id, string value)
        {
            Id = id;
            Value = value;
        }
    }

    public struct StringSortKey
        : IComparable<StringSortKey>
    {
        public int Length;
        public int Index;

        public StringSortKey(int length, int index)
        {
            Length = length;
            Index = index;
        }

        public int CompareTo(StringSortKey other)
        {
            // NOTE: 長さの降順で比較
            int lengthComparison = other.Length.CompareTo(Length);
            if (lengthComparison != 0)
            {
                return lengthComparison;
            }

            // NOTE: 同じ場合はインデックスの昇順で比較
            return Index.CompareTo(other.Index);
        }
    }

    public class MasterStringCompiler
        : IMasterStringCompiler
    {
        [MenuItem("Test/Test")]
        private static void A()
        {
            unsafe
            {
                var a = "test";
                Debug.Log(a);
                var c = Encoding.Unicode.GetByteCount(a);
                Debug.Log(c);

                {
                    var s = UnsafeConvert.StringToPtr(a);
                    var d = UnsafeConvert.PtrToString(s);
                    Debug.Log(d);
                }
                    {
                        string prototype = "PROTECTED";
                        IntPtr stringMT = typeof(string).TypeHandle.Value;

                        // プロトタイプから「生のアドレス」を取得
                        IntPtr protoPtr = *(IntPtr*)Unsafe.AsPointer(ref prototype);
                        // 本物の SyncBlock (MTの8バイト手前) をコピーして使い回す
                        long realSyncBlock = *(long*)(protoPtr - 8);

                        // 2. メモリ確保 (アライメントを確実にするため NativeMemory を使用)
                        // 構造: SyncBlock(8) + MT(8) + Length(4) + Data(可変)
                        int bufferSize = 128;
                        byte* rawBuffer = stackalloc byte[bufferSize];

                        // 3. 8バイトオフセットした位置を string の開始点にする
                        byte* stringStart = rawBuffer + 8;

                        // [Offset -8]: 本物の SyncBlock を書き込む
                        *(long*)(stringStart - 8) = realSyncBlock;

                        // [Offset 0]: Method Table
                        *(IntPtr*)(stringStart + 0) = stringMT;

                        // [Offset 8]: Length (4文字)
                        int charCount = 4;
                        *(int*)(stringStart + 8) = charCount;

                        // [Offset 12]: Char Data (UTF-16)
                        char* data = (char*)(stringStart + 12);
                        data[0] = 'D';
                        data[1] = 'O';
                        data[2] = 'N';
                        data[3] = 'E';
                        data[4] = '\0'; // 念のため

                        // 4. 強制代入
                        // 参照の参照を作ってから As で型をすり替える
                        IntPtr tempPtr = (IntPtr)stringStart;
                        string fakeString = Unsafe.As<IntPtr, string>(ref tempPtr);

                        // 5. 実行（ここで落ちる場合は、WriteLine内部の型チェックで弾かれている）
                        // 試しに、最も負荷の低い操作で確認
                        Debug.Log(fakeString == null);
                        if (fakeString != null)
                        {
                            Debug.Log(fakeString.Length);
                        }
                        if (fakeString != null && fakeString.Length == 4)
                        {
                            Debug.Log(fakeString);
                        }
                    }
            }
        }

        public class StringLinkTable
            : IDisposable
        {
            private struct Position
            {
                public long Offset;
                public int Length;
                public Position(long offset, int length)
                {
                    Offset = offset;
                    Length = length;
                }
            }

            private string m_directoryPath;
            private Encoding m_encoding;
            private NativeList<Position> m_positions;

            public StringLinkTable(string directoryPath, Encoding encoding)
            {
                m_directoryPath = directoryPath;
                m_encoding = encoding;
                m_positions = new(Allocator.Temp);
            }

            ~StringLinkTable()
            {
                DisposeInternal(false);
            }

            public void Dispose()
            {
                DisposeInternal(true);
                GC.SuppressFinalize(this);
            }

            private void DisposeInternal(bool disposing)
            {
                m_directoryPath = null;

                if (m_encoding is not null)
                {
                    m_encoding = null;
                }

                m_positions.Dispose();
                m_positions = default;
            }

            public IEnumerable<StringSource> Scan(IEnumerable<StringSource> source)
            {
                using var _sw = new StreamWriter(m_directoryPath + "/1_scan.txt");
                var _encorder = m_encoding.GetEncoder();

                var _position = 0L;

                foreach (var _element in source)
                {
                    var _count = _encorder.GetByteCount(_element.Value, false);
                    m_positions.Add(new(_position, _count));
                    _position += _count;

                    _sw.Write(_element.Value);

                    yield return _element;
                }
            }

            public void Compress(NativeArray<StringSortKey> sortKeys)
            {
                //AsyncReadManager.OpenFileAsync

                using var _sw = new StreamWriter(m_directoryPath + "/2_compress.txt");

                using var _fr = new DirectFileReader(m_directoryPath + "/1_scan.txt");

                var _source = m_positions.AsArray();
                //var _destination = new NativeArray<Position>(
                //    _source.Length
                //    , Allocator.Temp
                //    , NativeArrayOptions.UninitializedMemory
                //);

                var _confrict = new List<(int src, int dst)>();

                var _span = sortKeys.AsReadOnlySpan();
                for (int i = 0; i < _span.Length; ++i)
                {
                    var _baseKey = _span[i];
                    var _basePosition = _source[_baseKey.Index];

                    var _base = _fr.GetSpan(_basePosition.Offset, _basePosition.Length);

                    for (int j = i + 1; j < _span.Length; ++j)
                    {
                        var _searchKey = _span[j];
                        var _searchPosition = _source[_searchKey.Index];

                        var _search = _fr.GetSpan(_searchPosition.Offset, _searchPosition.Length);

                        // NOTE: 同文字列長の場合
                        if (_baseKey.Length == _searchKey.Length)
                        {
                            // NOTE: 同文字列か判定
                            if (_base.SequenceEqual(_search))
                            {
                                _confrict.Add((_searchKey.Index, _baseKey.Index));
                                Debug.Log($"Equal {_searchKey.Index},{_searchKey.Length} : {_baseKey.Index},{_searchKey.Length}");
                            }

                            continue;
                        }

                        var _index = _base.IndexOf(_search);
                        // NOTE: 抱合判定
                        if (-1 != _index)
                        {
                            _confrict.Add((_searchKey.Index, _baseKey.Index));
                            Debug.Log($"Contain {_searchKey.Index},{_searchKey.Length} : {_baseKey.Index},{_searchKey.Length} :: {_index}");
                        }
                    }
                }
            }
        }

        [MenuItem("MasterString/Compile")]
        public static void Compile()
        {
            var _compiler = new MasterStringCompiler();
            _compiler.Execute(Enumerable());
        }

        [MenuItem("MasterString/Test")]
        public static async void Test()
        {
            var _id = Progress.Start("MasterString Compile");

            await Task.Delay(1000);

            Progress.Report(_id, 0.1f, "Good");

            await Task.Delay(3000);

            Progress.Finish(_id);
        }

        private static IEnumerable<StringSource> Enumerable()
        {
            var _array = new StringSource[]
            {
                new(new(1), "Hello"),
                new(new(2), "What's up?"),
                new(new(3), "Pain!"),
                new(new(4), "You made me a, You made me a, Beliver, Beliver"),
                new(new(5), "Oh, ooh"),
                new(new(6), "First things first"),
                new(new(7), "Third"),
                new(new(8), "The master of my sea"),
                new(new(9), "Hey"),
                new(new(10), "Yo solo"),
                new(new(11), "Argolithm"),
                new(new(12), "Third"),
                new(new(13), "Yo"),
            };

            var _max = 0;
            foreach (var _element in _array)
            {
                yield return _element;

                var _length = _element.Id.EditorOnlyValue;
                if (_max < _length)
                {
                    _max = _length;
                }
            }

            _max = _max == 0
                ? 1
                : Mathf.FloorToInt(Mathf.Log10(_max)) + 1;

            using var _fs = new StreamWriter("Assets/TableSource.txt");

            foreach (var _element in _array)
            {
                var _id = _element.Id.EditorOnlyValue;
                var _text = _element.Value;

                _fs.WriteLine($"{_id.ToString($"D{_max}")}:{_text}");
            }
        }

        public void Execute(IEnumerable<StringSource> source)
        {
            using var _table = new StringLinkTable("Assets/Cache", Encoding.UTF8);

            using var _identifiers = new NativeList<StringIdentifier>(Allocator.Temp);
            using var _sortKeys = new NativeList<StringSortKey>(Allocator.Temp);

            var _count = 0;
            foreach (var _element in _table.Scan(source))
            {
                _identifiers.Add(_element.Id);
                _sortKeys.Add(new(_element.Value.Length, _count));
                ++_count;
            }

            // NOTE: 長さの降順、同じ長さの場合はインデックスの昇順でソート
            var _array = _sortKeys.AsArray();
            _array.Sort();

            {
                using var _fs = new StreamWriter("Assets/TableSorted.txt");
                for (int i = 0; i < _array.Length; ++i)
                {
                    var _v = _array[i];
                    _fs.WriteLine($"{_v.Length:0000} {_v.Index:0000}");
                }
            }

            AssetDatabase.Refresh();

            _table.Compress(_array);
        }
    }
}
