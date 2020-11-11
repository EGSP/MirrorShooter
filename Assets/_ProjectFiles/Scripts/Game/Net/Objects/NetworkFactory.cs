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
        public static void SpawnForAll(GameObject obj, UserConnection uc = null) 
        {
            if (LaunchInfo.IsServer)
            {
                if (uc != null && uc.Connection != null)
                {
                    NetworkServer.Spawn(obj.gameObject, uc.Connection);
                }
                else
                {
                    NetworkServer.Spawn(obj.gameObject);   
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
                    
                    NetworkServer.Spawn(obj.gameObject, uc.Connection);
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
    }
}