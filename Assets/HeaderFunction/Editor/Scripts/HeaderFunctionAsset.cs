using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    public class HeaderFunctionAsset
        : ScriptableObject
    {
        [SerializeReference] private HeaderFunctionDrawer[] m_drawers;

        public HeaderFunctionDrawer[] Drawers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_drawers;
        }

        public void Initialize(HeaderFunctionDrawer[] drawers)
        {
            m_drawers = drawers;
        }
    }
}

