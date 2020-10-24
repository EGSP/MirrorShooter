using System;
using Game.Net;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerEntity : NetworkBehaviour
    {
        [BoxGroup("Body")]
        [OdinSerialize] public PlayerBodyEntity BodyEntity { get; private set; }
        [BoxGroup("Body/Movement")]
        [OdinSerialize] public PlayerMoveModule MoveModule { get; private set; }

        [BoxGroup("Camera")] 
        [OdinSerialize] public PlayerCameraEntity CameraEntity { get; private set; }

        [FoldoutGroup("User", 100)]
        [SyncVar] public User owner;

       

        private void Awake()
        {
            var rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
                throw new NullReferenceException();
            
            MoveModule.Initialize(rigidBody);
        }
    }
}