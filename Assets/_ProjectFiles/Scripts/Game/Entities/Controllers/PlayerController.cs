using System;
using System.Collections.Generic;
using Game.Configuration;
using Game.Net.Objects;
using Mirror;
using UnityEngine;

namespace Game.Entities.Controllers
{
    public class PlayerController : DualNetworkBehaviour
    {
        [SerializeField] private Vector2 mouseSensivity;

        [SyncVar(hook = nameof(OnEntityChanged))]
        public uint playerEntityId;

        private PlayerEntity _playerEntity;

        // SERVER_SIDE
        private PlayerInputManager _playerInputManager;

        public override void AwakeOnServer()
        {
            if (_playerInputManager == null)
                _playerInputManager = new PlayerInputManager();
        }

        public override void UpdateOnClient()
        {
            if (_playerEntity == null)
                return;
            
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
            
            // Здесь нужно формировать лист из нажаты= отжатых кнопок и отправлять его.

            var newDown = new List<int>();
            var newUp = new List<int>();
            
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                newUp.Add((int)KeyCode.LeftShift);
            }
            
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                newDown.Add((int)KeyCode.LeftShift);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                newUp.Add((int) KeyCode.Space);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                newDown.Add((int) KeyCode.Space);
            }

            CmdHandleUpKeys(newUp);
            CmdHandleDownKeys(newDown);
        }

        [Server]
        public override void LateUpdateOnServer()
        { 
            _playerInputManager.LateUpdate(Time.deltaTime);
        }

        [Client]
        private void RotateCamera(float rotationX)
        {
            _playerEntity.CameraEntity.RotateX(rotationX);
        }

        [Client]
        private void RotateBody(float rotationY)
        {
            _playerEntity.BodyEntity.RotateY(rotationY);
        }

        [Command(ignoreAuthority = true)]
        private void CmdMoveBody(float horDelta, float verDelta)
        {
            Debug.Log("CMD_MOVE");
            _playerEntity.MoveModule.Move(horDelta, verDelta);
        }

        [Command(ignoreAuthority = true)]
        private void CmdHandleUpKeys(List<int> keyCodes)
        {
            for (int i = 0; i < keyCodes.Count; i++)
            {
                _playerInputManager.NewUp((KeyCode)keyCodes[i]);
            }
        }

        [Command(ignoreAuthority = true)]
        private void CmdHandleDownKeys(List<int> keyCodes)
        {
            Debug.Log($"Key count {keyCodes.Count}");
            for (int i = 0; i < keyCodes.Count; i++)
            {
                Debug.Log($"{(KeyCode)keyCodes[i]}");
                _playerInputManager.NewDown((KeyCode)keyCodes[i]);
            }
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

            playerEntity.SetInput(_playerInputManager);
            // _playerEntity.netIdentity.hasAuthority = true;
        }
        
    }
}