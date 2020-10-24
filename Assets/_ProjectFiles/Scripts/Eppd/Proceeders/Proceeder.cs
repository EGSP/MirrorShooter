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
    public class Proceeder<TDataBlock> where TDataBlock: DataBlock
    {
        [OdinSerialize]
        /// <summary>
        /// Процессы обработки блока данных.
        /// </summary>
        protected List<IProcess<TDataBlock>> Processes;


        /// <summary>
        /// Обработка данных через процессы.
        /// Блок данных изменяется по ссылке.
        /// </summary>
        /// <returns></returns>
        public virtual TDataBlock Proceed(TDataBlock inData)
        {
            for (var i = 0; i < Processes.Count; i++)
            {
                Processes[i].Process(inData);
            }

            return inData;
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
