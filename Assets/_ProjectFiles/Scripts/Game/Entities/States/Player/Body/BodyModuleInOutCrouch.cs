using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player.Body
{
    public enum InOut
    {
        In = 0,
        Out = 1
    }
    
    /// <summary>
    /// Переход в приседание.
    /// </summary>
    public class BodyModuleInOutCrouch : BodyModuleState
    {
        private readonly int _inOut;
        
        /// <param name="inOrOut">0 - in, 1 - out</param>
        public BodyModuleInOutCrouch(PlayerBodyModule module, InOut inOut) : base(module)
        {
            _inOut = (int) inOut;
            _crouchingTime = 0f;
        }

        private float _crouchingTime;

        public override BodyModuleState UpdateOnServer(float deltaTime)
        {
            if (_crouchingTime > Module.InOutCrouchTime)
            {
                if (NextState != null)
                    return NextState;
                else
                    return null;
            }
            
            // Debug.Log($"Crouch {(InOut)_inOut}");
            
            _crouchingTime += deltaTime;

            // На какой мы точке.
            var opacity = Mathf.Clamp(_crouchingTime / Module.InOutCrouchTime, 0,1);
            Module.CurrentCrouchOpacity = opacity;

            opacity = Mathf.Abs(_inOut - opacity);
            
            var height = Module.InOutCrouch.Get(opacity);
            
            // Debug.Log($"Height {height}");

            Module.Collider.height = height;

           
                Module.Collider.center =
                    new Vector3(0, Mathf.Lerp(Module.CrouchY.y, Module.CrouchY.x, opacity), 0);
            
            
            // var boundsSize = Module.Collider.bounds.size;
            // Module.Collider.bounds.Expand(new Vector3(boundsSize.x,height,boundsSize.z));
            
            return this;
        }
    }
}