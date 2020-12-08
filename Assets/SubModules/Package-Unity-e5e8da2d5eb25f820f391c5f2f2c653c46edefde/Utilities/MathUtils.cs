using UnityEngine;

namespace Egsp.Utils.MathUtilities
{
    public static class MathUtils
    {
        // Smoothing rate dictates the proportion of source remaining after one second
        // Same as DampExp.
        public static float DampPow(float source, float target, float smoothing, float dt)
        {
            return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
        }
        
        // Smoothing rate dictates the proportion of source remaining after one second
        // Same as DampPow.
        public static float DampExp(float source, float target, float lambda, float dt)
        {
            return Mathf.Lerp(source, target, 1 - Mathf.Exp(-lambda * dt));
        }
        
        // Smoothing rate dictates the proportion of source remaining after one second
        // Same as DampExp.
        public static Vector3 DampPow(Vector3 source, Vector3 target, float smoothing, float dt)
        {
            var x = DampPow(source.x, target.x, smoothing, dt);
            var y = DampPow(source.y, target.y, smoothing, dt);
            var z = DampPow(source.z, target.z, smoothing, dt);
            return new Vector3(x,y,z);
        }
        
        // Smoothing rate dictates the proportion of source remaining after one second
        // Same as DampPow.
        public static Vector3 DampExp(Vector3 source, Vector3 target, float lambda, float dt)
        {
            var x = DampExp(source.x, target.x, lambda, dt);
            var y = DampExp(source.y, target.y, lambda, dt);
            var z = DampExp(source.z, target.z, lambda, dt);
            return new Vector3(x,y,z);
        }
    }
}