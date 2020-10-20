using Game.Net;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerEntity : NetworkBehaviour
    {
        [SerializeField] private Transform cameraPlace;
        
        [SyncVar] public User owner;

        
        /// <summary>
        /// Устанавливает камеру в объект носитель.
        /// </summary>
        public void SetCamera(Camera camera)
        {
            camera.transform.SetParent(cameraPlace,true);
            camera.transform.localRotation = Quaternion.identity;
            camera.transform.localPosition = Vector3.zero;
        }
    }
}