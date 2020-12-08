using Egsp.Core.Ui;
using UnityEngine;

namespace Egsp.Core.Ui
{
    public static class ViewFactory
    {
        public readonly static string PathToUi = "Prefabs/UI/";
        
        /// <summary>
        /// Возвращает инстанс.
        /// </summary>
        public static TView LoadAndInstantiateView<TView>(string name, bool enableOnInstantiate = true)
            where TView : SerializedView
        {
            var prefab = Resources.Load<TView>(PathToUi + name);

            var inst = UnityEngine.Object.Instantiate(prefab);

            if (enableOnInstantiate)
            {
                inst.Enable();
            }
            else
            {
                inst.Disable();
            }

            return inst;
        }

        public static void UnloadView(SerializedView instance, bool destroy = true)
        {
            if (destroy)
            {
                instance.Unloading = true;
                UnityEngine.Object.Destroy(instance);
            }
            
            Resources.UnloadUnusedAssets();
        }
        
        public static void UnloadFromView(SerializedView instance)
        {
            Resources.UnloadUnusedAssets();
        }
        
    }
}