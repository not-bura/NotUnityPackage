using UnityEngine;

namespace NotBura.Packages
{
    public static class NotGizmosUtility
    {
        public static Matrix4x4 Matrix(NotGizmosDrawContext baseContext_, NotGizmosDrawContext thisContext_)
        {
            var _baseTransform = baseContext_.Transform;
            var _thisTransform = thisContext_.Transform;

            if (_thisTransform != null)
            {
                return _thisTransform.localToWorldMatrix * thisContext_.Matrix;
            }

            if (_baseTransform != null)
            {
                return _baseTransform.localToWorldMatrix * baseContext_.Matrix * thisContext_.Matrix;
            }

            return baseContext_.Matrix * thisContext_.Matrix;
        }
    }
}
