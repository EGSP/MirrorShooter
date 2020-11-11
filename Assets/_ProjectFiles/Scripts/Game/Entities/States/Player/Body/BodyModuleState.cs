using Game.Entities.Modules;

namespace Game.Entities.States.Player.Body
{
    public class BodyModuleState : LogicState<BodyModuleState, PlayerBodyModule>
    {
        public BodyModuleState(PlayerBodyModule module) : base(module)
        {
            
        }

        public override BodyModuleState ReturnThis()
        {
            return this;
        }
    }
}