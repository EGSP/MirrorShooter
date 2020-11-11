using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player.Body
{
    public class BodyModuleInSprint : BodyModuleState
    {
        private float _inSprintTime;
        
        public BodyModuleInSprint(PlayerBodyModule module) : base(module)
        {
            _inSprintTime = 0;
        }

        public override BodyModuleState UpdateOnServer(float deltaTime)
        {
            if (_inSprintTime > Module.InSprintTime)
            {
                if (NextState != null)
                    return NextState;
                else
                    return null;
            }
            
            _inSprintTime += deltaTime;
            
            // На какой мы точке.
            var opacity = Mathf.Clamp(_inSprintTime / Module.InSprintTime, 0,1);
            Module.CurrentCrouchOpacity = opacity;
            
            var height = Module.InSprint.Get(opacity);

            Module.Collider.height = height;

           
            Module.Collider.center =
                new Vector3(0, Mathf.Lerp(Module.CrouchY.y, Module.CrouchY.x, opacity), 0);

            return this;
        }
    }
}