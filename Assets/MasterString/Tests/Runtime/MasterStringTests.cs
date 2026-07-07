using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NotBura.Packages
{
    public class MasterStringTests
    {
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

            var _provider = new InMemoryMasterStringProvider(_state);

            MasterStringAPI.RegisterMasterStringProvider(_provider);

            MasterStringAPI.Dispose();
        }
    }

}
