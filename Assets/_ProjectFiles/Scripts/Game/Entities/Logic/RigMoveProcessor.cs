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

        /// <summary>
        /// Просчитывает направление движние.
        /// </summary>
        public readonly QueueProceeder<Vector3> DirectionProceeder;

        /// <summary>
        /// Просчитывает модификаторы данных ходьбы.
        /// </summary>
        public readonly Proceeder<MoveData> ModifierProceeder;

        private readonly MoveData _moveData;
        
        public RigMoveProcessor(RigidBodyData rigidBodyData, MoveData moveData) : base()
        {
            _rigidBodyData = rigidBodyData;
            _moveData = moveData;
            
            InsertData(moveData);
            InsertData(rigidBodyData);
            
            DirectionProceeder = new QueueProceeder<Vector3>((_old, _new) => _old + _new);
            ModifierProceeder = new Proceeder<MoveData>();
            
            AppendProcess(new MoveProcess());
        }

        public override void Tick()
        {
            var body = _rigidBodyData.Rigidbody.transform;
            
            // Получение направления движения
            _moveData.Direction = DirectionProceeder.Proceed();

            // Подключение модификаторов.
            ModifierProceeder.Proceed(_moveData);
            
            // Debug.Log($"{modifiedMoveData.Speed}");
            
            // Запуск процессов
            base.Tick();
            DataReadAvailable();
            
            ModifierProceeder.Reset();
        }

        public void AddMoveDataModifier(IProcess<MoveData> modifier)
        {
            Debug.Log("APPEND MODIFIER");
            ModifierProceeder.AppendProcess(modifier);
        }

        public void RemoveMoveDataModifier<TProcess>()
            where TProcess : IProcess<MoveData>
        {
            ModifierProceeder.RemoveProcesses<TProcess>();
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
    public class MoveData : DataBlock, ICloneable<MoveData>
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

        public MoveData Clone()
        {
            var clone = new MoveData();
            clone.Speed = Speed;
            clone.Direction = Direction;
            clone.FixedDeltaTime = FixedDeltaTime;
            return clone;
        }

        public void Accept(MoveData clone)
        {
            Speed = clone.Speed;
            Direction = clone.Direction;
            FixedDeltaTime = clone.FixedDeltaTime;
        }
    }

    public class MoveProcess : ProcessBase
    {
        private MoveData _moveData;
        private RigidBodyData _rigidBodyData;

        public override void Process()
        {
            // Debug.Log("MOVE_PROCESS");
            if (DataNullOrDisposed(_moveData, _rigidBodyData))
                return;
            
            // Debug.Log($"MOVE_PROCESS_({_moveData.Speed})");
            _rigidBodyData.Rigidbody.MovePosition(_rigidBodyData.Rigidbody.position +
                                                  _moveData.Direction * _moveData.Speed * _moveData.FixedDeltaTime);
        }

        public override void LookForData(Dictionary<Type, DataBlock> data)
        {
            TryGetData(data, ref _moveData);
            TryGetData(data, ref _rigidBodyData);
        }
    }

    public class RunDataModifier : ProcessBase<MoveData>
    {
        private readonly float _speedModifier;
        public RunDataModifier(float speedModifier)
        {
            _speedModifier = speedModifier;
        }
        
        public override void Process(MoveData dataBlock)
        {
            dataBlock.Speed *= _speedModifier;
        }
    }

    public class JumpDataModifier : ProcessBase<MoveData>
    {
        public float FlySpeed { get; set; }
        public JumpDataModifier(float flySpeed)
        {
            FlySpeed = flySpeed;
        }
        
        public override void Process(MoveData dataBlock)
        {
            dataBlock.Speed = FlySpeed;
        }
    }
}