using System;
using Game.Configuration;
using Game.Net.Exceptions;
using Game.Net.Mirrors;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Net.Objects
{
    public static class NetworkFactory
    {
        /// <param name="uc">Соединение, которому будут присвоены права на объект.</param>
        public static void SpawnForAll(GameObject obj, UserConnection uc = null) 
        {
            if (LaunchInfo.IsServer)
            {
                // Объект будет видем только тем, кто загрузился на сцену.
                AddLoadedVisibility(obj);
                
                if (uc != null && uc.Connection != null)
                {
                    NetworkServer.Spawn(obj, uc.Connection);
                }
                else
                {
                    NetworkServer.Spawn(obj);   
                }
                return;
            }
            
            throw new NotServerException();
        }
        
        public static void SpawnForConnection(GameObject obj, UserConnection uc)
        {
            if (LaunchInfo.IsServer)
            {
                if (uc.Connection != null)
                {
                    var identity = obj.GetComponent<NetworkIdentity>();
                    var visibility = identity.ReplaceVisibility<OneConnectionVisibility>();
                    visibility.SpecificConnectionToClient = uc.Connection as NetworkConnectionToClient;
                    
                    NetworkServer.Spawn(obj, uc.Connection);
                    return;
                }
                
                throw new InvalidUserConnection(uc);
            }    
            
            throw new NotServerException();
        }
        
        public static T InstantiateForAll<T>(T prefab) where T : MonoBehaviour
        {
            if (LaunchInfo.IsServer)
            {
                var inst = Object.Instantiate(prefab);
                return inst;
            }
            
            throw new NotServerException();
        }
        
        public static T InstantiateForConnection<T>(T prefab, UserConnection uc) where T : MonoBehaviour
        {
            if (LaunchInfo.IsServer)
            {
                var inst = Object.Instantiate(prefab);
                return inst;
            }    
            
            throw new NotServerException();
        }

        public static void Destroy(GameObject obj)
        {
            if (LaunchInfo.IsServer)
            {
                if (CheckIdentity(obj))
                {
                    NetworkServer.Destroy(obj);
                }
                else
                {
                    UnityEngine.Object.Destroy(obj);    
                }

                return;
            }
            
            throw new NotServerException();
        }
        
        private static bool CheckIdentity(GameObject obj)
        {
            if (obj.GetComponent<NetworkIdentity>() != null)
                return true;

            return false;
        }

        private static void AddLoadedVisibility(GameObject obj)
        {
            var identity = obj.GetComponent<NetworkIdentity>();
            identity.ReplaceVisibility<UserSceneStateVisibility>();
        }
    }
}