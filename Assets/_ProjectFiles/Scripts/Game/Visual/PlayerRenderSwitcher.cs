using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Visual
{
    public class PlayerRenderSwitcher : SerializedMonoBehaviour
    {
        public Transform root;
        
        /// <summary>
        /// Объекты, которые отображаются при виде со стороны.
        /// </summary>
        public GameObject[] thirdPersonObjects;

        [BoxGroup("Animations")]
        [OdinSerialize] public Animator Animator { get; private set; }
        [BoxGroup("Animations")]
        [OdinSerialize] public RuntimeAnimatorController ThirdPersonAnimatorController { get; private set; }
        [BoxGroup("Animations")]
        [OdinSerialize] public RuntimeAnimatorController FirstPersonAnimatorController { get; private set; }
        
        private GameObject _handsAsset;
        private GameObject _handsInstance;

        /// <summary>
        /// Дополнительно отключает рендер тела.
        /// </summary>
        public void RenderHands()
        {
            if (Animator == null)
            {
                Debug.LogWarning("Animator on Switcher is null");
                return;
            }

            Animator.runtimeAnimatorController = FirstPersonAnimatorController;
            
            for (var i = 0; i < thirdPersonObjects.Length; i++)
            {
                thirdPersonObjects[i].SetActive(false);
            }

            _handsAsset = Resources.Load<GameObject>("Player/Hands");

            if(_handsInstance != null)
                Destroy(_handsInstance);

            _handsInstance = Instantiate(_handsAsset, Vector3.zero, Quaternion.identity);
        }

        private void OnDestroy()
        {
            if (_handsAsset != null)
            {
                // Все инстансы сами удаляются.
                Resources.UnloadAsset(_handsAsset);
            }
        }
    }
}