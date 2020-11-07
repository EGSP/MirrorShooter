using Game.Entities.Modules;

namespace Game.Entities.States.Player.Animation
{
    public class AnimationModuleState : LogicState<AnimationModuleState, PlayerAnimationModule>
    {
        public AnimationModuleState(PlayerAnimationModule module) : base(module)
        {
        }

        public override AnimationModuleState ReturnThis()
        {
            return this;
        }
    }
}