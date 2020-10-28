using System;
using Game.Configuration;
using Game.Net.Objects;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerController : DualNetworkBehaviour
    {
        [SerializeField] private Vector2 mouseSensivity;

        [SyncVar(hook = nameof(OnEntityChanged))]
        public uint playerEntityId;

        private PlayerEntity _playerEntity;

        public override void UpdateOnClient()
        {
            // Mouse rotation Input
            float rotationY = Input.GetAxis("Mouse X") * mouseSensivity.x; // Вращение по горизонтали
            float rotationX = -Input.GetAxis("Mouse Y") * mouseSensivity.y; // Вращение по вертикали

            float horizontalDelta = Input.GetAxisRaw("Horizontal");
            float verticalDelta = Input.GetAxisRaw("Vertical");
            
            if(rotationX != 0)
                RotateCamera(rotationX);
            
            if(rotationY != 0)
                RotateBody(rotationY);
            
            if(horizontalDelta != 0 || verticalDelta != 0)
                CmdMoveBody(horizontalDelta, verticalDelta);
        }

        [Client]
        private void RotateCamera(float rotationX)
        {
            _playerEntity.CameraEntity.RotateX(rotationX);
            // _playerEntity.CameraEntity.CmdRotateX(rotationX);
        }

        [Client]
        private void RotateBody(float rotationY)
        {
            _playerEntity.BodyEntity.RotateY(rotationY);
            // _playerEntity.BodyEntity.CmdRotateY(rotationY);
        }

        // Заменить на модуль 
        [Command(ignoreAuthority = true)]
        private void CmdMoveBody(float horDelta, float verDelta)
        {
            Debug.Log("CMD_MOVE");
            _playerEntity.MoveModule.Move(horDelta, verDelta);
        }
        
        public void OnEntityChanged(uint _, uint newEntity)
        {
            if (NetworkIdentity.spawned.TryGetValue(newEntity, out NetworkIdentity identity))
            {
                _playerEntity = identity.GetComponent<PlayerEntity>();
                // _playerEntity.BodyEntity.Rigidbody.isKinematic = true;
                _playerEntity.netIdentity.hasAuthority = true;
                _playerEntity.netIdentity.isLocalRepresenter = true;

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
            // _playerEntity.netIdentity.hasAuthority = true;
        }
        
    }
}