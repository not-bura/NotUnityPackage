using System;
using UnityEngine;

namespace NotBura.Packages
{
    public interface ITMProMasterStringSource
    {
        public ReadOnlySpan<char> GetText();
    }

    [Serializable]
    public sealed class TMProMasterStringSource
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
    public sealed class TMProValueStringSource
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
}
