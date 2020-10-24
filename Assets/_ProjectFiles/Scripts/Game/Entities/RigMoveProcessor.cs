using System;
using System.Collections.Generic;
using System.Linq;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Proceeders;
using Gasanov.Eppd.Processes;
using Gasanov.Eppd.Processors;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Processors
{
    public class RigMoveProcessor : Processor
    {

        private readonly RigidBodyData _rigidBodyData;
        private readonly MoveData _moveData;

        /// <summary>
        /// Просчитывает направление движние.
        /// </summary>
        public readonly QueueProceeder<Vector3> DirectionProceeder;
        
        public RigMoveProcessor(RigidBodyData rigidBodyData, MoveData moveData) : base()
        {
            InsertData(rigidBodyData);
            InsertData(moveData);

            _rigidBodyData = rigidBodyData;
            _moveData = moveData;

            DirectionProceeder = new QueueProceeder<Vector3>(
                (_old, _new) => _old + _new);
            
            AppendProcess(new MoveProcess());
        }

        public override void Tick()
        {
            var body = _rigidBodyData.Rigidbody.transform;
            
            // Получение направления движения
            _moveData.Direction = DirectionProceeder.Proceed();
            
            // Запуск процессов
            base.Tick();
            DataReadAvailable();
        }
    }
    
    public class RigidBodyData : DataBlock 
    {
        public RigidBodyData(Rigidbody rigidbody)
        {
            Rigidbody = rigidbody;
        }

        /// <summary>
        /// Физическое тело.
        /// </summary>
        public readonly Rigidbody Rigidbody;
    }

    [Serializable]
    public class MoveData : DataBlock
    {
        /// <summary>
        /// Скорость передвижения.
        /// </summary>
        [OdinSerialize]
        public float Speed { get; set; }
        
        /// <summary>
        /// Промежуток времени между кадрами физического движка.
        /// </summary>
        public float FixedDeltaTime { get; set; }
        
        /// <summary>
        /// Вектор движения.
        /// </summary>
        public Vector3 Direction { get; set; }
    }
    
    public class MoveProcess : ProcessBase
    {   
        private MoveData _moveData;
        private RigidBodyData _rigidBodyData;

        public override void Process()
        {
            if (DataNullOrDisposed(_moveData, _rigidBodyData))
                return;
            
            _rigidBodyData.Rigidbody.MovePosition(_moveData.Direction * _moveData.Speed * _moveData.FixedDeltaTime);
        }

        public override void LookForData(Dictionary<Type, DataBlock> data)
        {
            TryGetData(data, ref _moveData);
            TryGetData(data, ref _rigidBodyData);
        }
    }
}