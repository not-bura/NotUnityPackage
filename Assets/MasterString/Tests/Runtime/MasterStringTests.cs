using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Unity.PerformanceTesting;
using Unity.PerformanceTesting.Measurements;
using UnityEngine;

namespace NotBura.Packages
{
    internal sealed class MasterStringTests
    {
        private string[] m_texts;

        [OneTimeSetUp]
        public void SetUp()
        {
            m_texts = new string[]
            {
                "こんにちはこんばんはおはようございます",
                "今は朝の十時なので夕飯のランチを楽しみたいと思いませんか？",
                "長野県の千駄ヶ谷駅から徒歩120kmほど進んだところにバス停がありました",
                "今日はエレベストからソリで下ってきたのですが、思ったよりも早く着きましたね",
                "what's up bro, you lock so bones.",
            };
        }

        [Test]
        public void Simple()
        {
            var a = UTF8Helper.GetByteCount(m_texts);
            var b = UTF8Helper.GetByteCountTrue(m_texts);

            Debug.Log($"{a} {b}");

            Assert.AreEqual(a, b);
        }


        [Test, Performance]
        public void SpeedByteCountTest()
        {
            var measurement = Measure.Method(Impl);
            Run(measurement);

            void Impl()
            {
                var result = UTF8Helper.GetByteCount(m_texts);
            }
        }

        [Test, Performance]
        public void SpeedByteCountNewTest()
        {
            var measurement = Measure.Method(Impl);
            Run(measurement);

            void Impl()
            {
                var result = UTF8Helper.GetByteCountTrue(m_texts);
            }
        }

        private void Run(MethodMeasurement measurement)
        {
            measurement
                .WarmupCount(50)
                .IterationsPerMeasurement(5000)
                .MeasurementCount(50)
                .Run();
        }

        public void MenuItem()
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
