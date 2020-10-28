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
        public readonly Proceeder<MoveData> MoveProceeder;

        /// <summary>
        /// Процесс, отвечающий за движение.
        /// </summary>
        private readonly MoveProcess _moveProcess;
        private readonly MoveData _moveData;
        
        public RigMoveProcessor(RigidBodyData rigidBodyData, MoveData moveData) : base()
        {
            _rigidBodyData = rigidBodyData;
            _moveData = moveData;
            
            InsertData(rigidBodyData);
            
            DirectionProceeder = new QueueProceeder<Vector3>((_old, _new) => _old + _new);
            MoveProceeder = new Proceeder<MoveData>();
            
            _moveProcess = new MoveProcess();
            _moveProcess.LookForData(DataBlocks);
        }

        public override void Tick()
        {
            var body = _rigidBodyData.Rigidbody.transform;
            
            // Получение направления движения
            _moveData.Direction = DirectionProceeder.Proceed();

            // Подключение модификаторов.
            var modifiedMoveData = MoveProceeder.Proceed(_moveData);
            
            _moveProcess.Process(modifiedMoveData);
            
            // Запуск процессов
            base.Tick();
            DataReadAvailable();
        }

        public void AddMoveDataModifier(IProcess<MoveData> modifier)
        {
            MoveProceeder.AppendProcess(modifier);
        }

        public void RemoveMoveDataModifier<TProcess>()
            where TProcess : IProcess<MoveData>
        {
            MoveProceeder.RemoveProcesses<TProcess>();
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
    }

    public class MoveProcess : ProcessBase<MoveData>
    {
        private RigidBodyData _rigidBodyData;

        public override void Process(MoveData dataBlock)
        {
            // Debug.Log("MOVE_PROCESS");
            if (DataNullOrDisposed(dataBlock, _rigidBodyData))
                return;
            
            // Debug.Log($"MOVE_PROCESS_({dataBlock.Direction}, {dataBlock.Speed})");
            _rigidBodyData.Rigidbody.MovePosition(_rigidBodyData.Rigidbody.position +
                                                  dataBlock.Direction * dataBlock.Speed * dataBlock.FixedDeltaTime);
        }


        public override void LookForData(Dictionary<Type, DataBlock> data)
        {
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
}