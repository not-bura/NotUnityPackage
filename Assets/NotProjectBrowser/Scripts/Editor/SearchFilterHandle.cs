using System;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public struct SearchFilterHandle
    {
        public static readonly Type WrapperType = Type.GetType("UnityEditor.SearchFilter,UnityEditor");

        private static readonly RWField<object, string[]> s_foldersField = WrapperType
            .GetNonPublicInstanceRWField<object, string[]>("m_Folders");

        private static readonly StaticMethod<object, string> s_createSearchFilterFromStringMethod = WrapperType
            .GetNonPublicStaticMethod<object, string>("CreateSearchFilterFromString", WrapperType);

        private object m_instance;

        public object Instance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_instance;
        }

        public string[] Folders
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => s_foldersField.Get(m_instance);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => s_foldersField.Set(m_instance, value);
        }

        private SearchFilterHandle(object instance)
        {
            m_instance = instance;
        }

        public static SearchFilterHandle CreateSearchFilterFromString(string searchText)
        {
            var _result = s_createSearchFilterFromStringMethod.Invoke(searchText);

            return new(_result);
        }
    }
}
