﻿using System;
using Game.Entities.Modules;
using Game.Processors;
using Gasanov.Eppd.Proceeders;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    [Serializable]
    public class PlayerMoveModule : LogicModule
    {
        [OdinSerialize] public float MoveSpeed { get; private set; } = 5f;
        
        // Нужна для передачи значения времени между кадрами.
        [OdinSerialize] private MoveData _moveData;
        
        /// <summary>
        /// Физическое тело для перемещения.
        /// </summary>
        private Rigidbody _rigidbody;

        /// <summary>
        /// Процессор передвижения.
        /// </summary>
        [OdinSerialize] public RigMoveProcessor MoveProcessor { get; private set; }

        
        public void SetRigidBody(Rigidbody rig)
        {
            _rigidbody = rig;
            
            _moveData = new MoveData(){Speed = MoveSpeed};
            MoveProcessor = new RigMoveProcessor(new RigidBodyData(_rigidbody), _moveData);
        }

        public override void FixedUpdateOnServer()
        {
            _moveData.FixedDeltaTime = Time.fixedDeltaTime;
            MoveProcessor.Tick();
        }

        /// <summary>
        /// Передвижение объекта с помощью двух направлений.
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void Move(float horizontal, float vertical)
        {
            var forwardBack = vertical * _rigidbody.transform.forward;
            var rightLeft = horizontal * _rigidbody.transform.right;
            
            MoveProcessor.DirectionProceeder.Add(forwardBack+rightLeft);
        }
    }
}