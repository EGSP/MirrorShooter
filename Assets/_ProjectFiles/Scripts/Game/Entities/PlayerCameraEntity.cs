using System;
using Game.Net.Objects;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerCameraEntity : DualNetworkBehaviour
    {
        /// <summary>
        /// Корневой объект камеры, к которому она привязана.
        /// </summary>
        [BoxGroup("Objects")][OdinSerialize]
        public Transform CameraRoot { get; private set; }
        /// <summary>
        /// Объект камеры, он привязан к корневому объекту.
        /// </summary>
        [BoxGroup("Objects")][OdinSerialize]
        public Camera CameraObject { get; private set; }
        
        /// <summary>
        /// Ограничение поворота камеры по оси X.
        /// Ограничение работает относительно любого начального поворота. 
        /// </summary>
        [BoxGroup("Settings")]
        [OdinSerialize] public float CameraVerticalRotationClamp { get; private set; }
        
        
        /// <summary>
        /// Текущий угол поворота камеры.
        /// </summary>
        private float _cameraVerticalRotation;
        /// <summary>
        /// Кешированное значение поворота камеры.
        /// </summary>
        private Quaternion _cachedCameraRotation;

        public override void AwakeOnClient()
        {
            if(CameraRoot == null)
                throw new NullReferenceException();
            
            _cachedCameraRotation = CameraRoot.localRotation;
        }

        public override void AwakeOnServer()
        {
            if(CameraRoot == null)
                throw new NullReferenceException();
            
            _cachedCameraRotation = CameraRoot.localRotation;
        }

        /// <summary>
        /// Установка новой камеры. Старая камера будет удалена.
        /// Если новая камера null, то просто ничего не произойдет.
        /// </summary>
        public void SetCamera(Camera cameraObject)
        {
            if (CameraRoot == null)
                return;
            
            if (cameraObject == null)
                return;
            
            if (CameraObject != null)
                Destroy(CameraObject.gameObject);
            
            CameraObject = cameraObject;
            
            // Присоедининие к корневому объекту.
            CameraObject.transform.SetParent(CameraRoot,false);
            CameraObject.transform.localRotation = Quaternion.identity;
            CameraObject.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Вращение на стороне клиента.
        /// </summary>
        public void RotateX(float deltaRotationX)
        {
            // Поворот камеры относительно тела.
            _cameraVerticalRotation += deltaRotationX;
            // Ограничение поворота по вертикали.
            _cameraVerticalRotation = Mathf.Clamp(_cameraVerticalRotation,
                -CameraVerticalRotationClamp, CameraVerticalRotationClamp); 
            
            // Новое вращение камеры.
            _cachedCameraRotation = Quaternion.Euler(_cameraVerticalRotation, 0, 0);
            // Вращаем корневой объект.
            CameraRoot.localRotation = _cachedCameraRotation;
        }

        /// <summary>
        /// Поворот камеры по оси Х.
        /// </summary>
        [Command(ignoreAuthority = true)]
        public void CmdRotateX(float deltaRotationX)
        {
            // Поворот камеры относительно тела.
            _cameraVerticalRotation += deltaRotationX;
            // Ограничение поворота по вертикали.
            _cameraVerticalRotation = Mathf.Clamp(_cameraVerticalRotation,
                -CameraVerticalRotationClamp, CameraVerticalRotationClamp); 
            
            // Новое вращение камеры.
            _cachedCameraRotation = Quaternion.Euler(_cameraVerticalRotation, 0, 0);
            // Вращаем корневой объект.
            CameraRoot.localRotation = _cachedCameraRotation;
        }
    }
}