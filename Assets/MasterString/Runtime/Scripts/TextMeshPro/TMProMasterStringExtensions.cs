using System.Buffers;
using TMPro;

namespace NotBura.Packages
{
    public static class TMProMasterStringExtensions
    {
//#if NOT_BURA_MASTER_STRING_ENABLE_TEXTMESHPRO
        public static void SetText(this TMP_Text target, MasterString source)
        {
            var _provider = MasterStringAPI.MasterStringProvider;
            var _text = _provider.Resolve(source.Id);

            var _pool = ArrayPool<char>.Shared;
            var _array = _pool.Rent(_text.Length);

            try
            {
                _text.CopyTo(_array);
                target.SetText(_array);
            }
            finally
            {
                _pool.Return(_array);
            }
        }
//#endif
    }
}
