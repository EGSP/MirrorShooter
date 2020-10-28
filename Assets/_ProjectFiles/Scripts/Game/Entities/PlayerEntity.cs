using System;
using Game.Entities.Modules;
using Game.Net;
using Game.Net.Objects;
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
        [BoxGroup("Body/Movement")]
        [OdinSerialize] public PlayerMoveModule MoveModule { get; private set; }

        [BoxGroup("Camera")] 
        [OdinSerialize] public PlayerCameraEntity CameraEntity { get; private set; }

        [FoldoutGroup("User", 100)]
        [SyncVar] public User owner;

        public override void AwakeOnClient()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            BodyEntity.SetRigidBody(rigidBody);
        }

        public override void AwakeOnServer()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            BodyEntity.SetRigidBody(rigidBody);
            
            MoveModule.Initialize(this);
            MoveModule.SetRigidBody(rigidBody);
        }
    }
}