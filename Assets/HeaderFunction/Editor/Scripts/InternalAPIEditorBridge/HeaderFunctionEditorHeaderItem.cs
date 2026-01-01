using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NotBura.Packages
{
    public static class HeaderFunctionEditorHeaderItem
    {
        public static Func<Rect, Object[], bool> Handler;

        [EditorHeaderItem(typeof(Object))]
        private static bool EditorHeaderItem(Rect rectangle, Object[] targetObjets)
        {
            if (Handler == null)
            {
                return false;
            }
            
            return Handler.Invoke(rectangle, targetObjets);
        }
    }
}
