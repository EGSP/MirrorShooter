using System.Collections.Generic;
using System.Linq;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    public class PlayerAnimationModule : LogicModule
    {
        [SerializeField] private FlexNetworkAnimator _networkAnimator;

        // /// <summary>
        // /// Все хеши анимаций.
        // /// </summary>
        // private Dictionary<string, int> _animationHashes;

        public override void Initialize(IUnityMethodsHook hook)
        {
            base.Initialize(hook);

            // _animationHashes = new Dictionary<string, int>();
            // var clips = _networkAnimator.Animator.runtimeAnimatorController.animationClips;
            //
            // // Запоминаем хеши.
            // for (var i = 0; i < clips.Length; i++)
            // {
            //     var clip = clips[i];
            //     _animationHashes.Add(clip.name, Animator.StringToHash(clip.name));
            // }
            
        }


        /// <summary>
        /// Проигрывает анимацию. Без сетевого аниматора ничего не включит.
        /// </summary>
        public void Play(string animationString)
        {
            var hash = Animator.StringToHash(animationString);

            if (_networkAnimator != null)
                _networkAnimator.SetTrigger(hash);
        }
    }
}