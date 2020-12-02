using System;
using System.Collections.Generic;
using Game.Configuration;
using Game.Net.Objects;
using Game.Views.Client.Session;
using Gasanov.Core.Mvp;
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

        /// <summary>
        /// Список нажатых клавиш (полон только во время обновления)
        /// </summary>
        protected List<int> down = new List<int>();
        /// <summary>
        /// Список отжатых клавиш (полон только во время обновления)
        /// </summary>
        protected List<int> up = new List<int>();

        public override void AwakeOnClient()
        {
            CreatePlayerUi();
            CommonAwake();
        }

        private void CommonAwake()
        {
            GlobalInput.SetCharacterMode();
        }

        public override void AwakeOnServer()
        {
            if (PlayerInputManager == null)
                PlayerInputManager = new PlayerInputManager();
            
            CommonAwake();
        }

        public override void UpdateOnClient()
        {
            if (PlayerEntity == null)
                return;

            // TODO: Выделить для всех контроллеров интерфейс. GlobalInput просто включал бы и выключал группы.
            // TODO: Для плеер менеджера сделать метод, который бы сбрасывал нажатие кнопок.
            if (GlobalInput.Mode == GlobalInput.InputMode.Interface)
                return;
            
            // Mouse rotation Input
            float rotationY = Input.GetAxis("Mouse X") * mouseSensivity.x; // Вращение по горизонтали
            float rotationX = -Input.GetAxis("Mouse Y") * mouseSensivity.y; // Вращение по вертикали

            float horizontalDelta = Input.GetAxisRaw("Horizontal");
            float verticalDelta = Input.GetAxisRaw("Vertical");
            
            if(rotationX != 0)
                RotateCamera(rotationX);

            if (rotationY != 0)
            {
                // Debug.Log(rotationY);
                RotateBody(rotationY);
            }
            // Здесь нужно формировать лист из нажаты - отжатых кнопок и отправлять его.

            DefineKeys();
            
            CmdHandleDownKeys(down);
            CmdHandleUpKeys(up);
            
            down.Clear();
            up.Clear();
        }

        protected void DefineKeys()
        {
            DefineKeyCodeState(KeyCode.W);
            DefineKeyCodeState(KeyCode.S);
            DefineKeyCodeState(KeyCode.A);
            DefineKeyCodeState(KeyCode.D);
            
            DefineKeyCodeState(KeyCode.LeftShift);
            
            DefineKeyCodeState(KeyCode.Space);
            
            DefineKeyCodeState(KeyCode.LeftControl);
        }
        
        /// <summary>
        /// Заносит клавишу в список состояний.
        /// </summary>
        protected void DefineKeyCodeState(KeyCode code)
        {
            if (Input.GetKeyUp(code))
            {
                up.Add((int) code);
            }

            if (GlobalInput.Mode == GlobalInput.InputMode.Interface)
                return;
            
            if (Input.GetKeyDown(code))
            {
                down.Add((int) code);
            }
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
            PlayerEntity.BodyModule.RotateY(rotationY);
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
            // Debug.Log("ENTITY CHANGED");
            if (NetworkIdentity.spawned.TryGetValue(newEntity, out NetworkIdentity identity))
            {
                // Debug.Log($"ENTITY FINDED with netID {identity.netId}");
                PlayerEntity = identity.GetComponent<PlayerEntity>();
                PlayerEntity.netIdentity.isLocalRepresenter = true;

                // PlayerEntity.GetComponent<Rigidbody>().useGravity = false;

                if (LaunchInfo.LaunchMode == LaunchModeType.Client)
                    PlayerEntity.CameraEntity.SetCamera(Camera.main, PlayerEntity.BodyModule.CameraTarget);
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
            // playerEntity.netIdentity.hasAuthority = true;
        }

        [Server]
        public void SetPlayerEntityCamera()
        {
            PlayerEntity.CameraEntity.SetCamera(Camera.main, PlayerEntity.BodyModule.CameraTarget);
        }

        public void CreatePlayerUi()
        {
            var view = ViewFactory.LoadAndInstantiateView<ClientQuickMenuView>("quick_menu",
                false);
        }
        
    }
}