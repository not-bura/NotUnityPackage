using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NotBura.Packages
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NotGizmosDrawerAttribute
        : Attribute
    {
        private readonly string m_name;

        public string Name
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_name;
        }

        public NotGizmosDrawerAttribute(string name)
        {
            m_name = name;
        }
    }

    public interface INotGizmosWire
    {
        public bool IsWire { get; }
    }

    [Serializable]
    public abstract class BaseNotGizmosDrawer
    {
        [SerializeField] protected bool m_enabled = true;
        [SerializeField] protected NotGizmosDrawContext m_context = new();

        public bool Enabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_enabled;
        } 

        public BaseNotGizmosDrawer()
        {
        }

        public abstract void Draw(NotGizmosDrawContext context, NotGizmosDrawStates state);
    }
}
