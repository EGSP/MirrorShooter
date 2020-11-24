using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Game.Net.Resources
{
    public static class NetworkResources
    {
        // public static IResourcesRegister ResourcesRegister;
        
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
        //
        // public static T Get<T>(string id)
        // {
        //     
        // }
    }

    // public interface IResourcesRegister
    // {
    //     void Register(List<Resource> resources);
    //     
    //     T Get<T>(string id);
    // }
    //
    // public sealed class ServerResourcesRegister : IResourcesRegister
    // {
    //     private Dictionary<string, Resource> registeredResources = new Dictionary<string, Resource>();
    //     
    //     public void Register(List<Resource> resources)
    //     {
    //         for (var i = 0; i < resources.Count; i++)
    //         {
    //             var resource = resources[i];
    //             if (!registeredResources.ContainsKey(resource.Id))
    //             {
    //                 registeredResources.Add(resource.Id,resource);
    //             }
    //         }
    //     }
    //
    //     public T Get<T>(string id)
    //     {
    //         
    //     }
    // }
    //
    public interface IResourceLoader
    {
        List<GameObject> LoadPrefabs();
    }
    //
    // public sealed class Resource
    // {
    //     public readonly string Id;
    //     public readonly GameObject GameObject;
    //
    //     public Resource(string id, GameObject gameObject)
    //     {
    //         Id = id;
    //         GameObject = gameObject;
    //     }
    // }
}