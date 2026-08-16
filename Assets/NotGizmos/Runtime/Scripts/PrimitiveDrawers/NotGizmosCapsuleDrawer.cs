using System;
using UnityEngine;

namespace NotBura.Packages
{
    [Serializable]
    public sealed class NotGizmosCapsuleDrawer
        : BaseNotGizmosDrawer
    {
        private static Vector3[] s_points = Get();

        static Vector3[] Get()
        {
            var _split = 30;
            var _points = new Vector3[_split * 8];

            var angle = 180.0f / _split * Mathf.Deg2Rad;

            {
                var _start = 0;
                var _length = _split + 1;

                var _xUp = _points.AsSpan(_start, _length);
                var _zUp = _points.AsSpan(_start + _split * 2, _length);
                for (int i = 0; i < _xUp.Length; ++i)
                {
                    var _cos = Mathf.Cos(angle * i);
                    var _sin = Mathf.Sin(angle * i) + 0.5f;

                    _xUp[i].x = _cos;
                    _xUp[i].y = _sin;

                    _zUp[i].z = _cos;
                    _zUp[i].y = _sin;
                }
            }

            {
                var _start = _split + 1;
                var _length = (_split * 2) - (_split + 1);

                var _xDown = _points.AsSpan(_start, _length);
                var _zDown = _points.AsSpan(_start + _split * 2, _length);
                for (int i = 0; i < _xDown.Length; ++i)
                {
                    var _cos = -Mathf.Cos(angle * i);
                    var _sin = -Mathf.Sin(angle * i) - 0.5f;

                    _xDown[i].x = _cos;
                    _xDown[i].y = _sin;

                    _zDown[i].z = _cos;
                    _zDown[i].y = _sin;
                }
            }

            {
                var _start = _split * 4;
                var _length = (_split * 2);

                var _upSide = _points.AsSpan(_start, _length);
                var _downSide = _points.AsSpan(_start + _split * 2, _length);

                for (int i = 0; i < _upSide.Length; ++i)
                {
                    var _cos = Mathf.Cos(angle * i);
                    var _sin = Mathf.Sin(angle * i);

                    _upSide[i].x = _cos;
                    _upSide[i].y = 0.5f;
                    _upSide[i].z = _sin;

                    _downSide[i].x = _cos;
                    _downSide[i].y = -0.5f;
                    _downSide[i].z = _sin;
                }
            }

            return _points;
        }

        public override void Draw(NotGizmosDrawContext context, NotGizmosDrawStates state)
        {
            Gizmos.DrawLineStrip(s_points.AsSpan(000, 60), true);
            Gizmos.DrawLineStrip(s_points.AsSpan(060, 60), true);
            Gizmos.DrawLineStrip(s_points.AsSpan(120, 60), true);
            Gizmos.DrawLineStrip(s_points.AsSpan(180, 60), true);
        }
    }
}
