using System;
using UnityEngine;

namespace Egsp.Other
{
    [Serializable]
    public class CurveHandler
    {
        [SerializeField] private AnimationCurve curve;

        /// <summary>
        /// Получение значения по позиции точки на оси x.
        /// </summary>
        public float Get(float pointX)
        {
            if (curve == null)
                return 0;

            return curve.Evaluate(pointX);
        }
    }
}