using System;
using Game.Net.Objects;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    // Этот класс будет переписан!!!!!!
    public class PlayerController : DualNetworkBehaviour
    {
        
        [SerializeField] private Vector2 mouseSensivity;
        [SerializeField] private float clampCameraVerticalRotation; // Ограничение поворота камеры по оси X
        
        [SyncVar(hook = nameof(OnEntityChanged))]
        public uint playerEntityId;

        private PlayerEntity _playerEntity;

        private float _cameraVerticalRotation; // Значение угла поворота камеры
        private Quaternion _newCameraRotation;
        private Quaternion _newBodyRotation;

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
                CmdRotateCamera(rotationX);
            
            if(rotationY != 0)
                CmdRotateBody(rotationY);
        }

        protected override void OnServer()
        {
            // throw new NotImplementedException();
        }

        [Command(ignoreAuthority = true)]
        private void CmdRotateCamera(float rotationX)
        {
            if (_playerEntity == null && _playerEntity.cameraTransform == null)
                return;
            
            Debug.Log("CMD_CAMERA");
            // Поворот камеры относительно тела
            _cameraVerticalRotation += rotationX;
            _cameraVerticalRotation = Mathf.Clamp(_cameraVerticalRotation,
                -clampCameraVerticalRotation, clampCameraVerticalRotation); // Ограничение поворота
            
            _newCameraRotation = Quaternion.Euler(_cameraVerticalRotation, 0, 0); // Новое вращение камеры
            _playerEntity.cameraTransform.localRotation = 
                _newCameraRotation; // Применяем вращение
        }

        [Command(ignoreAuthority = true)]
        private void CmdRotateBody(float rotationY)
        {
            if (_playerEntity == null && _playerEntity.bodyTransform == null)
                return;
            
            Debug.Log("CMD_BODY");
            // Новое вращение тела
            _newBodyRotation *= Quaternion.Euler(0, rotationY, 0);
            
            // Поворот тела
            _playerEntity.bodyTransform.rotation = 
                _newBodyRotation;
        }
        
        // УЖАСНЫЙ КОД, НУЖНО ПЕРЕДЕЛАТЬ
        
        public void OnEntityChanged(uint _, uint newEntity)
        {
            if (NetworkIdentity.spawned.TryGetValue(newEntity, out NetworkIdentity identity))
            {
                _playerEntity = identity.GetComponent<PlayerEntity>();
                   
                _playerEntity.SetCamera(Camera.main);
                _playerEntity.GetComponent<NetworkTransformChild>().target = _playerEntity.cameraTransform;
                
                _newCameraRotation = _playerEntity.cameraTransform.localRotation;
                _newBodyRotation = _playerEntity.bodyTransform.rotation;
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
            
            _playerEntity.SetCamera(Camera.main);
            _playerEntity.GetComponent<NetworkTransformChild>().target = _playerEntity.cameraTransform;
            
            _newCameraRotation = _playerEntity.cameraTransform.localRotation;
            _newBodyRotation = _playerEntity.bodyTransform.rotation;
        }
        
    }
}