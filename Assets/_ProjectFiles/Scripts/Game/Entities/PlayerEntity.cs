using System;
using Game.Net;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerEntity : NetworkBehaviour
    {
        public Transform cameraPlace;
        public Transform bodyTransform;
        public Transform cameraTransform;
        
        
        [SyncVar] public User owner;

        
        /// <summary>
        /// Устанавливает камеру в объект носитель.
        /// </summary>
        public void SetCamera(Camera camera)
        {
            camera.transform.SetParent(cameraPlace,true);
            camera.transform.localRotation = Quaternion.identity;
            camera.transform.localPosition = Vector3.zero;
            cameraTransform = camera.transform;
        }
    }
}