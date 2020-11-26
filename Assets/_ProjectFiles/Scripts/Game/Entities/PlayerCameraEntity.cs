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
        
        [OdinSerialize][ReadOnly]
        public Camera CameraComponent { get; private set; }
        
        /// <summary>
        /// Ограничение поворота камеры по оси X.
        /// Ограничение работает относительно любого начального поворота. 
        /// </summary>
        [BoxGroup("Settings")]
        [OdinSerialize] public float CameraVerticalRotationClamp { get; private set; }
        [BoxGroup("Settings")]
        [OdinSerialize][PropertyRange(0,1)] public float FollowRate { get; private set; }
        
        
        /// <summary>
        /// Текущий угол поворота камеры.
        /// </summary>
        private float _cameraVerticalRotation;
        
        /// <summary>
        /// Кешированное значение поворота камеры.
        /// </summary>
        private Quaternion _cachedCameraRotation;

        private Transform _target;
        

        public override void LateUpdateOnClient()
        {
            if (CameraComponent == null)
                return;
            
            Follow();
        }

        public override void LateUpdateOnServer()
        {
            if (CameraComponent == null)
                return;
            
            Follow();
        }

        private void Follow()
        {
            if (_target == null)
                return;
            
            var tr = CameraComponent.transform;
            
            tr.position = Vector3.Slerp(tr.position, _target.position, FollowRate);

            // Поворот относительно тела.
            tr.rotation = _target.rotation * _cachedCameraRotation;
        }

        /// <summary>
        /// Установка новой камеры. Старая камера будет удалена.
        /// Если новая камера null, то просто ничего не произойдет.
        /// </summary>
        public void SetCamera(Camera cameraComponent, Transform target)
        {
            if (cameraComponent == null)
                return;
            
            if (CameraComponent != null)
                Destroy(CameraComponent.gameObject);
            
            CameraComponent = cameraComponent;
            
            CameraComponent.transform.SetParent(null);
            CameraComponent.transform.rotation = Quaternion.identity;
            CameraComponent.transform.position = Vector3.zero;

            _cachedCameraRotation = CameraComponent.transform.rotation;

            _target = target;
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
            CameraComponent.transform.rotation = _cachedCameraRotation;
        }
    }
}