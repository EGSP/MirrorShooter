using System;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Game.Net;
using Game.Net.Objects;
using Game.Sessions;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerEntity : DualNetworkBehaviour
    {
        [BoxGroup("Body")]
        [OdinSerialize] public PlayerBodyEntity BodyEntity { get; private set; }

        [BoxGroup("Camera")] 
        [OdinSerialize] public PlayerCameraEntity CameraEntity { get; private set; }
        
        [BoxGroup("Modules")]
        [OdinSerialize] public PlayerMoveModule MoveModule { get; private set; }
        
        [BoxGroup("Modules")]
        [OdinSerialize] public PlayerAnimationModule AnimationModule { get; private set; }

        [FoldoutGroup("User", 100)]
        [SyncVar] public User owner;
        
        // SERVER
        private PlayerInputManager _playerInputManager;
        
        public override void AwakeOnClient()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            rigidBody.useGravity = false;
            
        }
        
        public override void AwakeOnServer()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            ServerSession.singletone.AddPlayerEntity(this);
            
            MoveModule.Initialize(this);
            MoveModule.Setup(this, rigidBody);

            AnimationModule.Initialize(this);
        }
        


        public void SetInput(PlayerInputManager playerInputManager)
        {
            _playerInputManager = playerInputManager;

            MoveModule.PlayerInputManager = _playerInputManager;
        }
        

        [Button("Show movement info")]
        private void ShowMoveInfo()
        {
            if (MoveModule != null && GetComponent<Rigidbody>() != null)
            {
                Debug.Log($" Expected jump height: " +
                          $"{MoveModule.ExcpectedJumpHeight(GetComponent<Rigidbody>())}");
            }
        }
    }
}