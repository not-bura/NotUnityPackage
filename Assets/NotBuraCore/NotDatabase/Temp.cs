using NotBura.Core;
using System;
using System.Runtime.CompilerServices;

namespace NotBura.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeyAttibute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OffsetAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CharactorAttribute : Attribute
    {
        private uint m_size;

        public uint Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public CharactorAttribute(uint size)
        {
            m_size = size;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TextAttribute : Attribute
    {
    }
}

public class NotStaticDatabase
{
    // NOTE: フィールド名を指定する方法を提供してもいい
    private NotTable<TestStatic> m_testStaticTable;
}

public unsafe struct TestStatic
{

}

public unsafe struct TestDynamic
{
    public int Integer;
    public char* Name;
}

public class Usecase
{
    public void Execute()
    {
        using var systemHandle = new NotDatabaseSystemHandle("A");
        using var db = new NotDynamicDatabase(systemHandle);
        using var tests = db.CreateTable<TestDynamic>("Tests");

        tests.Insert(new TestDynamic());
    }
}
