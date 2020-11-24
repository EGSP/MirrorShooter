using System.Collections.Generic;
using System.Linq;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;
using Game.Entities.States.Player.Animation;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    public class PlayerAnimationModule : LogicModule<AnimationModuleState, PlayerAnimationModule>
    {
        [SerializeField] private FlexNetworkAnimator _networkAnimator;

        protected override void DefineStates()
        {
            DefineState(typeof(AnimationModuleState).Name, () =>
            {
                return new AnimationModuleState(this);
            });
        }


        /// <summary>
        /// Проигрывает анимацию. Без сетевого аниматора ничего не включит.
        /// </summary>
        public void Play(string animationString)
        {
            var hash = Animator.StringToHash(animationString);

            if (_networkAnimator != null)
            {
                _networkAnimator.Animator.Play(hash);
            }

        }
    }
}