using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    public class MasterStringTests
    {
        [UnityEditor.MenuItem("MasterString/Test")]
        public static void MenuItem()
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

            var _provider = new MasterStringProvider(_state);

            MasterStringAPI.Register(_provider);

            MasterStringAPI.Dispose();
        }
    }

}
