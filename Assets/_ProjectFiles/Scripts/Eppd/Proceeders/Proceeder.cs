using System.Collections;
using System.Collections.Generic;
using Eppd.Processors;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processes;
using Gasanov.Extensions.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Gasanov.Eppd.Proceeders
{   
    public class Proceeder<TDataBlock> where TDataBlock: DataBlock, ICloneable<TDataBlock>
    {
        [OdinSerialize]
        /// <summary>
        /// Процессы обработки блока данных.
        /// </summary>
        protected List<IProcess<TDataBlock>> Processes = new List<IProcess<TDataBlock>>();


        /// <summary>
        /// Обработка данных через процессы.
        /// Блок данных клонируется, обрабатывается и возвращается в виде результата.
        /// </summary>
        /// <returns></returns>
        public virtual TDataBlock Proceed(TDataBlock inData)
        {
            var clone = inData.Clone();
            
            for (var i = 0; i < Processes.Count; i++)
            {
                Processes[i].Process(inData);
            }

            return clone;
        }
        
        /// <summary>
        /// Добавляет процесс в конец списка.
        /// </summary>
        public void AppendProcess(IProcess<TDataBlock> process)
        {
            Processes.Add(process);
        }

        /// <summary>
        /// Убирает все процессы определенного типа.
        /// </summary>
        public void RemoveProcesses<T>() where T : IProcess<TDataBlock>
        {
            Processes.RemoveByType<T>();
        }
        
    }
}
