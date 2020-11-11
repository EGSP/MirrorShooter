﻿using System.Collections;
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
        /// Временный клон.
        /// </summary>
        private TDataBlock _temporaryClone;

        /// <summary>
        /// Временная ссылка на изначальные данные.
        /// </summary>
        private TDataBlock _temporaryReference;
        
        /// <summary>
        /// Обработка данных через процессы.
        /// Блок данных клонируется, обрабатывается и возвращается в виде результата.
        /// </summary>
        /// <returns></returns>
        public virtual void Proceed(TDataBlock inData)
        {
            _temporaryReference = inData;
            _temporaryClone = inData.Clone();
            
            for (var i = 0; i < Processes.Count; i++)
            {
                Processes[i].Process(_temporaryReference);
            }
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
        
        /// <summary>
        /// Устанавливает изначальные настройки для блока данных.
        /// </summary>
        public void Reset()
        {
            _temporaryReference.Accept(_temporaryClone);
        }
    }
}
