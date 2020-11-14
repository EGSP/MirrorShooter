using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Game.Net.Resources
{
    public static class NetworkResources
    {
        public static void LoadAndRegister(IResourceLoader resourceLoader)
        {
            if (resourceLoader == null)
                return;
            
            var prefabs = resourceLoader.LoadPrefabs();

            for (var i = 0; i < prefabs.Count; i++)
            {
                ClientScene.RegisterPrefab(prefabs[i]);
            }
        }
    }

    public interface IResourceLoader
    {
        List<GameObject> LoadPrefabs();
    }
}