using System;
using System.Collections.Generic;
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
        [BoxGroup("Camera")] 
        [OdinSerialize] public PlayerCameraEntity CameraEntity { get; private set; }
        
        [BoxGroup("Modules")]
        [BoxGroup("Modules/Body")]
        [OdinSerialize] public PlayerBodyModule BodyModule { get; private set; }
        
        [BoxGroup("Modules/Movement")]
        [OdinSerialize] public PlayerMoveModule MoveModule { get; private set; }
        
        [BoxGroup("Modules/Animation")]
        [OdinSerialize] public PlayerAnimationModule AnimationModule { get; private set; }
        
        

        [FoldoutGroup("User", 100)]
        [SyncVar] public User owner;
        
        // SERVER
        private PlayerInputManager _playerInputManager;

        private Dictionary<string, LogicModule> definedModules = new Dictionary<string, LogicModule>(); 
        
        public override void AwakeOnClient()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            rigidBody.useGravity = false;
            
            BodyModule.Initialize(this);
            MoveModule.Initialize(this);
            MoveModule.Setup(this, rigidBody);
            AnimationModule.Initialize(this);
            
            definedModules.Add(BodyModule.ID,BodyModule);
            definedModules.Add(MoveModule.ID,MoveModule);
            definedModules.Add(AnimationModule.ID, AnimationModule);
        }

        public override void AwakeOnServer()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            ServerSession.Instance.AddPlayerEntity(this);
            
            BodyModule.Initialize(this);
            MoveModule.Initialize(this);
            MoveModule.Setup(this, rigidBody);
            AnimationModule.Initialize(this);

            BodyModule.OnStateChanged += OnStateChanged;
            MoveModule.OnStateChanged += OnStateChanged;
            AnimationModule.OnStateChanged += OnStateChanged;
        }
        
        

        public void SetInput(PlayerInputManager playerInputManager)
        {
            _playerInputManager = playerInputManager;

            MoveModule.PlayerInputManager = _playerInputManager;
            BodyModule.PlayerInputManager = _playerInputManager;
        }

        [TargetRpc]
        public void TargetSyncState(NetworkConnection connection, string module, string state)
        {
            Debug.Log($"STATE SYNC : {module} -> {state}");
            if (definedModules.ContainsKey(module))
            {
                var playerModule = definedModules[module];
                
                playerModule.RecognizeState(state);
            }
        }

        private void OnStateChanged(string module, string state)
        {
            if (state == null)
                return;
            
            if (state == "null")
                return;
            
            Debug.Log($"ONSTATECHANGED ({module}, {state})");
            TargetSyncState(netIdentity.connectionToClient, module, state);
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