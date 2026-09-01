using UnityEngine;

namespace NotBura.Core
{
    public static class NotColorUtility
    {
        public static Color Hue(float source)
        {
            source = Mathf.Clamp01(source);

            var _hue = source * 6.0f;
            var _sector = Mathf.FloorToInt(_hue);
            var _tween = _hue - _sector;
            var _tweenDown = 1.0f - _tween;
            var _tweenUp = 1.0f - _tweenDown;

            return _sector switch
            {
                0 => new(1.0f, Mathf.Clamp01(_tweenUp), 0.0f, 1.0f),
                1 => new(Mathf.Clamp01(_tweenDown), 1.0f, 0.0f, 1.0f),
                2 => new(0.0f, 1.0f, Mathf.Clamp01(_tweenUp), 1.0f),
                3 => new(0.0f, Mathf.Clamp01(_tweenDown), 1.0f, 1.0f),
                4 => new(Mathf.Clamp01(_tweenUp), 0.0f, 1.0f, 1.0f),
                5 => new(1.0f, 0.0f, Mathf.Clamp01(_tweenDown), 1.0f),
                _ => new(1.0f, 1.0f, 1.0f, 1.0f),
            };
        }

        public static Color HueRaw(float source)
        {
            var _hue = source * 6.0f;
            var _sector = Mathf.FloorToInt(_hue);
            var _tween = _hue - _sector;
            var _tweenDown = 1.0f - _tween;
            var _tweenUp = 1.0f - _tweenDown;

            return _sector switch
            {
                0 => new(1.0f, Mathf.Clamp01(_tweenUp), 0.0f, 1.0f),
                1 => new(Mathf.Clamp01(_tweenDown), 1.0f, 0.0f, 1.0f),
                2 => new(0.0f, 1.0f, Mathf.Clamp01(_tweenUp), 1.0f),
                3 => new(0.0f, Mathf.Clamp01(_tweenDown), 1.0f, 1.0f),
                4 => new(Mathf.Clamp01(_tweenUp), 0.0f, 1.0f, 1.0f),
                5 => new(1.0f, 0.0f, Mathf.Clamp01(_tweenDown), 1.0f),
                _ => new(1.0f, 1.0f, 1.0f, 1.0f),
            };
        }

        public static Color32 Hue32(float source)
        {
            source = Mathf.Clamp01(source);

            var _hue = source * 6.0f;
            var _sector = Mathf.FloorToInt(_hue);
            var _tween = _hue - _sector;
            var _tweenDown = 1.0f - _tween;
            var _tweenUp = 1.0f - _tweenDown;

            return _sector switch
            {
                0 => new(255, ToByte(_tweenUp), 0, 255),
                1 => new(ToByte(_tweenDown), 255, 0, 255),
                2 => new(0, 255, ToByte(_tweenUp), 255),
                3 => new(0, ToByte(_tweenDown), 255, 255),
                4 => new(ToByte(_tweenUp), 0, 255, 255),
                5 => new(255, 0, ToByte(_tweenDown), 255),
                _ => new(255, 255, 255, 255),
            };

            byte ToByte(float source)
            {
                if (source < 0.0f)
                {
                    return 0;
                }

                if (source > 1.0f)
                {
                    return 255;
                }

                return (byte)(source * 255.0f);
            }
        }

        public static Color32 Hue32Raw(float source)
        {
            var _hue = source * 6.0f;
            var _sector = Mathf.FloorToInt(_hue);
            var _tween = _hue - _sector;
            var _tweenDown = 1.0f - _tween;
            var _tweenUp = 1.0f - _tweenDown;

            return _sector switch
            {
                0 => new(255, ToByte(_tweenUp), 0, 255),
                1 => new(ToByte(_tweenDown), 255, 0, 255),
                2 => new(0, 255, ToByte(_tweenUp), 255),
                3 => new(0, ToByte(_tweenDown), 255, 255),
                4 => new(ToByte(_tweenUp), 0, 255, 255),
                5 => new(255, 0, ToByte(_tweenDown), 255),
                _ => new(255, 255, 255, 255),
            };

            byte ToByte(float value)
            {
                if (value < 0.0f)
                {
                    return 0;
                }

                if (value > 1.0f)
                {
                    return 255;
                }

                return (byte)(value * 255.0f);
            }
        }
    }
}
