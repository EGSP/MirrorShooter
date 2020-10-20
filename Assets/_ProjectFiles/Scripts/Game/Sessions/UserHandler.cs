using System;
using System.Collections.Generic;
using Game.Net;
using Gasanov.Utils.GameObjectUtilities;
using Mirror;
using UnityEngine;

namespace Game.Sessions
{
    public class UserHandler
    {
        /// <summary>
        /// Соединение, с которым связаны объекты.
        /// </summary>
        public readonly UserConnection UserConnection;
        
        public UserHandler(UserConnection uc)
        {
            if(uc == null)
                throw new NullReferenceException();
            
            UserConnection = uc;
            relatedGameObjects = new List<GameObject>();
        } 
        
        /// <summary>
        /// Связанные игровые объекты.
        /// </summary>
        private List<GameObject> relatedGameObjects;

        /// <summary>
        /// Добавление игрового объекта к пользователю.
        /// </summary>
        /// <param name="gameObject"></param>
        public void AddGameObject(GameObject gameObject)
        {
            if(gameObject != null && relatedGameObjects.Contains(gameObject) == false)
                relatedGameObjects.Add(gameObject);
        }

        /// <summary>
        /// Удаление объекта из списка.
        /// </summary>
        public void RemoveGameObject(GameObject gameObject)
        {
            if (relatedGameObjects.Contains(gameObject))
            {
                relatedGameObjects.Remove(gameObject);
            }
        }
        
        [Server]
        /// <summary>
        /// Этим методом удаляютс все объекты на стороне сервера и его клиентов. 
        /// </summary>
        public void DisposeAsServer()
        {
            foreach (var gameObject in relatedGameObjects)
            {
                if (gameObject != null)
                    NetworkServer.Destroy(gameObject);
            }
        }

        [Client]
        /// <summary>
        /// Этим методом удаляются только объекты на стороне клиента.
        /// </summary>
        public void DisposeAsClient()
        {
            foreach (var gameObject in relatedGameObjects)
            {
                if (gameObject != null)
                    GameObjectUtils.SafeDestroy(gameObject);
            }
        }
    }
}