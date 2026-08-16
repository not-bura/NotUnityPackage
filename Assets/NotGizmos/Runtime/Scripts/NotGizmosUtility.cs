using UnityEngine;

namespace NotBura.Packages
{
    public static class NotGizmosUtility
    {
        public static Matrix4x4 Matrix(NotGizmosDrawContext @base, NotGizmosDrawContext current)
        {
            var _baseTransform = @base.Transform;
            var _currentTransform = current.Transform;

            if (_currentTransform != null)
            {
                return _currentTransform.localToWorldMatrix * current.Matrix;
            }

            if (_baseTransform != null)
            {
                return _baseTransform.localToWorldMatrix * @base.Matrix * current.Matrix;
            }

            return @base.Matrix * current.Matrix;
        }
    }
}
