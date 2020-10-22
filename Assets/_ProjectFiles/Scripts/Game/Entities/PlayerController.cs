using System;
using Game.Configuration;
using Game.Net.Objects;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    // Этот класс будет переписан!!!!!!
    public class PlayerController : DualNetworkBehaviour
    {
        [SerializeField] private Vector2 mouseSensivity;

        [SyncVar(hook = nameof(OnEntityChanged))]
        public uint playerEntityId;

        private PlayerEntity _playerEntity;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnClient()
        {
            // Mouse rotation Input
            float rotationY = Input.GetAxis("Mouse X") * mouseSensivity.x; // Вращение по горизонтали
            float rotationX = -Input.GetAxis("Mouse Y") * mouseSensivity.y; // Вращение по вертикали
            
            if(rotationX != 0)
                RotateCamera(rotationX);
            
            if(rotationY != 0)
                RotateBody(rotationY);
        }

        protected override void OnServer()
        {
            // throw new NotImplementedException();
        }

        [Client]
        private void RotateCamera(float rotationX)
        {
            _playerEntity.CameraEntity.CmdRotateX(rotationX);
        }

        [Client]
        private void RotateBody(float rotationY)
        {
            _playerEntity.BodyEntity.CmdRotateY(rotationY);
        }
        
        public void OnEntityChanged(uint _, uint newEntity)
        {
            if (NetworkIdentity.spawned.TryGetValue(newEntity, out NetworkIdentity identity))
            {
                _playerEntity = identity.GetComponent<PlayerEntity>();

                if (LaunchInfo.LaunchMode == LaunchModeType.Client)
                    _playerEntity.CameraEntity.SetCamera(Camera.main);
            }
            else
            {
                Debug.Log("PlayerEntity не найден.");
            }
        }

        [Server]
        public void SetPlayerEntity(PlayerEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }
        
    }
}