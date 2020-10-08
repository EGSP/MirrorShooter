using UnityEngine;

namespace Gasanov.Utils.GameObjectUtilities
{
    public static class GameObjectUtils
    {
        public static void DestroyAllChildrens(Transform transform)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
        
        public static T SafeDestroy<T>(T obj) where T : UnityEngine.Object
        {
            if (Application.isEditor)
                UnityEngine.Object.DestroyImmediate(obj);
            else
                UnityEngine.Object.Destroy(obj);
     
            return null;
        }
        
        public static T SafeDestroyComponent<T>(T component) where T : Component
        {
            if (component != null)
                SafeDestroy(component.gameObject);
            return null;
        }
    }
}