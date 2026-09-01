using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace NotBura.Core.Tests
{
    public class UUIDTests
    {
        [Test, Performance]
        public void SimpleUUIDTest()
        {
            var source = "12345678-9999-4abc-dddd-222244445555";
            var a = UUID.FromCharSpan(source);
            var b = new UUID(0x1234_5678_9999_4abc, 0xdddd_2222_4444_5555);
            var c = a.ToString();

            Debug.Log(@$"{a}
{b}
{c}");
        }
    }
}
