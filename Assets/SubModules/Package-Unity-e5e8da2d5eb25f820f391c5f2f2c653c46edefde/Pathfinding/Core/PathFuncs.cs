using UnityEngine;

namespace Egsp.Core.Pathfinding
{
    public static class PathFuncs
    {
        /// <summary>
        /// Оценивает дистанцию между положением и текущей точкой.
        /// </summary>
        public static bool Vector3Continuation(Vector3 origin, Vector3 current)
        {
            var distance = (origin - current).sqrMagnitude;

            if (distance <= float.Epsilon)
            {
                return true;
            }

            return false;
        }
    }
}