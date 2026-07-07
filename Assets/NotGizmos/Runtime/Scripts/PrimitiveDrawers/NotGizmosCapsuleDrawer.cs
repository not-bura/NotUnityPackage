using System;
using System.Diagnostics;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public class NotGizmosCapsuleDrawer
        : BaseNotGizmosDrawer
    {
        private static Vector3[] s_points = Get();

        static Vector3[] Get()
        {
            var _cut = 30;
            var _points = new Vector3[_cut * 8];

            var a = 180.0f / _cut * Mathf.Deg2Rad;

            {
                var _start = 0;
                var _length = _cut + 1;

                var _xUp = _points.AsSpan(_start, _length);
                var _zUp = _points.AsSpan(_start + _cut * 2, _length);
                for (int i = 0; i < _xUp.Length; ++i)
                {
                    var _cos = Mathf.Cos(a * i);
                    var _sin = Mathf.Sin(a * i) + 0.5f;

                    _xUp[i].x = _cos;
                    _xUp[i].y = _sin;

                    _zUp[i].z = _cos;
                    _zUp[i].y = _sin;
                }
            }

            {
                var _start = _cut + 1;
                var _length = (_cut * 2) - (_cut + 1);

                var _d = _points.AsSpan(_start, _length);
                var _zDown = _points.AsSpan(_start + _cut * 2, _length);
                for (int i = 0; i < _d.Length; ++i)
                {
                    var _cos = -Mathf.Cos(a * i);
                    var _sin = -Mathf.Sin(a * i) - 0.5f;

                    _d[i].x = _cos;
                    _d[i].y = _sin;

                    _zDown[i].z = _cos;
                    _zDown[i].y = _sin;
                }
            }

            {
                var _start = _cut * 4;
                var _length = (_cut * 2);

                var _uSide = _points.AsSpan(_start, _length);
                var _dSide = _points.AsSpan(_start + _cut * 2, _length);

                for (int i = 0; i < _uSide.Length; ++i)
                {
                    var _cos = Mathf.Cos(a * i);
                    var _sin = Mathf.Sin(a * i);

                    _uSide[i].x = _cos;
                    _uSide[i].y = 0.5f;
                    _uSide[i].z = _sin;

                    _dSide[i].x = _cos;
                    _dSide[i].y = -0.5f;
                    _dSide[i].z = _sin;
                }
            }

            return _points;
        }

        [SerializeField] private float m;

        public override void Draw(NotGizmosDrawContext baseContext, NotGizmosDrawMode drawMode)
        {
            var v = Stopwatch.GetTimestamp();

            Gizmos.DrawLineStrip(s_points.AsSpan(000, 60), true);
            Gizmos.DrawLineStrip(s_points.AsSpan(060, 60), true);
            Gizmos.DrawLineStrip(s_points.AsSpan(120, 60), true);
            Gizmos.DrawLineStrip(s_points.AsSpan(180, 60), true);

            m *= 0.99f;
            m += (Stopwatch.GetTimestamp() - v) * 0.01f;
        }
    }
}
