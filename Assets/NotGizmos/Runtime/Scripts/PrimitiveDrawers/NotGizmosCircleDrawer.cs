using System.Buffers;
using UnityEditor;
using UnityEngine;

namespace NotBura.Packages
{
    public class NotGizmosCircleDrawer
        : BaseNotGizmosDrawer
    {
        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode drawMode)
        {
            var _cut = 60;
            var _points = (stackalloc Vector3[_cut]);

            var a = 360.0f / _cut * Mathf.Deg2Rad;

            for (int i = 0; i < _points.Length; ++i)
            {
                _points[i].x = Mathf.Cos(a * i);
                _points[i].z = Mathf.Sin(a * i);
            }

            Gizmos.DrawLineStrip(_points, true);
        }
    }
}
