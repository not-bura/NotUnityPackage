using System;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public sealed class NotGizmosCircleDrawer
        : BaseNotGizmosDrawer
    {
        public override void Draw(NotGizmosDrawContext context, NotGizmosDrawStates state)
        {
            var _split = 60;
            var _points = (stackalloc Vector3[_split]);

            var a = 360.0f / _split * Mathf.Deg2Rad;

            for (int i = 0; i < _points.Length; ++i)
            {
                _points[i].x = Mathf.Cos(a * i);
                _points[i].z = Mathf.Sin(a * i);
            }

            Gizmos.DrawLineStrip(_points, true);
        }
    }
}
