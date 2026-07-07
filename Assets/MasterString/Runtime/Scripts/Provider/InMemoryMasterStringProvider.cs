using System;
using System.Collections.Generic;
using System.Text;

namespace NotBura.Packages
{
    [Serializable]
    public sealed class MasterStringModel
    {
        public Encoding Encoding;
        public MasterStringLanguage Language;

        public List<Element> Table;

        [Serializable]
        public struct Element
        {
            public MasterStringLanguage Language;
            public List<Value> Elements;

            [Serializable]
            public struct Value
            {
                public StringIdentifier Id;
                public string Name;
            }
        }
    }

    [Serializable]
    public sealed class InMemoryMasterStringProvider
        : IMasterStrinProvider<StringIdentifier>
    {
        private MasterStringModel m_state;

        public InMemoryMasterStringProvider(MasterStringModel state)
        {
            m_state = state;
        }

        public ReadOnlySpan<char> Resolve(StringIdentifier id)
        {
            var _state = m_state;

            var _targetIndex = _state.Table.FindIndex(x => x.Language == _state.Language);
            if (-1 == _targetIndex)
            {
                return null;
            }

            var _table = _state.Table[_targetIndex];

            var _stringIndex = _table.Elements.FindIndex(x => x.Id == id);
            if (-1 == _stringIndex)
            {
                return null;
            }

            return _table.Elements[_stringIndex].Name;
        }

        public void Dispose()
        {
            m_state = null;
        }
    }
}
