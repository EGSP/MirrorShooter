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
        [SerializeField] protected Vector2 mouseSensivity;

        [SyncVar(hook = nameof(OnEntityChanged))]
        public uint playerEntityId;

        protected PlayerEntity PlayerEntity;

        // SERVER_SIDE
        protected PlayerInputManager PlayerInputManager;

        public override void AwakeOnServer()
        {
            if (PlayerInputManager == null)
                PlayerInputManager = new PlayerInputManager();
        }

        public override void UpdateOnClient()
        {
            if (PlayerEntity == null)
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
            PlayerInputManager.LateUpdate(Time.deltaTime);
        }

        protected void RotateCamera(float rotationX)
        {
            PlayerEntity.CameraEntity.RotateX(rotationX);
        }

        protected void RotateBody(float rotationY)
        {
            PlayerEntity.BodyEntity.RotateY(rotationY);
        }

        [Command(ignoreAuthority = true)]
        private void CmdMoveBody(float horDelta, float verDelta)
        {
            MoveBody(horDelta, verDelta);
        }

        [Server]
        protected void MoveBody(float horDelta, float verDelta)
        {
            PlayerEntity.MoveModule.Move(horDelta, verDelta);
        }

        [Command(ignoreAuthority = true)]
        private void CmdHandleUpKeys(List<int> keyCodes)
        {
            AddNewUp(keyCodes);
        }
        
        [Command(ignoreAuthority = true)]
        private void CmdHandleDownKeys(List<int> keyCodes)
        {
            AddNewDown(keyCodes);
        }

        
        [Server]
        protected void AddNewUp(List<int> keyCodes)
        {
            for (int i = 0; i < keyCodes.Count; i++)
            {
                PlayerInputManager.NewUp((KeyCode)keyCodes[i]);
            }
        }
        
        [Server]
        protected void AddNewDown(List<int> keyCodes)
        {
            for (int i = 0; i < keyCodes.Count; i++)
            {
                PlayerInputManager.NewDown((KeyCode)keyCodes[i]);
            }
        }
        
        
        
        public void OnEntityChanged(uint _, uint newEntity)
        {
            if (NetworkIdentity.spawned.TryGetValue(newEntity, out NetworkIdentity identity))
            {
                PlayerEntity = identity.GetComponent<PlayerEntity>();
                // _playerEntity.BodyEntity.Rigidbody.isKinematic = true;
                PlayerEntity.netIdentity.hasAuthority = true;
                PlayerEntity.netIdentity.isLocalRepresenter = true;

                if (LaunchInfo.LaunchMode == LaunchModeType.Client)
                    PlayerEntity.CameraEntity.SetCamera(Camera.main);
            }
            else
            {
                Debug.Log("PlayerEntity не найден.");
            }
        }

        [Server]
        public void SetPlayerEntity(PlayerEntity playerEntity)
        {   
            PlayerEntity = playerEntity;

            playerEntity.SetInput(PlayerInputManager);
            // _playerEntity.netIdentity.hasAuthority = true;
        }

        [Server]
        public void SetPlayerEntityCamera()
        {
            PlayerEntity.CameraEntity.SetCamera(Camera.main);
        }
        
    }
}