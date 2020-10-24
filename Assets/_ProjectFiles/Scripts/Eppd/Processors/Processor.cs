using System;
using System.Collections;
using System.Collections.Generic;
using Gasanov.Eppd.Processes;
using Gasanov.Extensions.Linq;
using Gasanov.Eppd.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gasanov.Eppd.Processors
{
    public class Processor
    {
        protected Processor()
        {
            Processes = new List<IProcess>();
            DataBlocks = new Dictionary<Type, DataBlock>();
        }

        /// <summary>
        /// Исполняемые процессы.
        /// </summary>
        [OdinSerialize]
        protected List<IProcess> Processes;

        /// <summary>
        /// Данные процессора.
        /// </summary>
        [OdinSerialize]
        protected Dictionary<Type, DataBlock> DataBlocks;
        
        /// <summary>
        /// Вызывается, когда данные могут быть прочитаны
        /// </summary>
        private event Action<Dictionary<Type, DataBlock>> OnDataReadAvailable = 
            delegate(Dictionary<Type, DataBlock> data) {  };

        /// <summary>
        /// Инициализирует процессор.
        /// </summary>
        public virtual void Init()
        {
            return;
        }

        /// <summary>
        /// Запуск нового прохода.
        /// </summary>
        public virtual void Tick()
        {
            // Выполнение всех процессов
            for (var i = Processes.Count; i < Processes.Count; i++)
            {
                Processes[i].Process();
            }
            DataReadAvailable();
        }

        /// <summary>
        /// Добавляет процесс в конец списка.
        /// </summary>
        /// <param name="process"></param>
        public void AppendProcess(IProcess process)
        {
            OnDataReadAvailable += process.LookForData;
            Processes.Add(process);
        }
        
        /// <summary>
        /// Убирает все процессы определенного типа.
        /// </summary>
        public void RemoveProcesses<T>() where T : class, IProcess
        {
            for (var i = Processes.Count - 1; i > -1; i--)
            {
                var process = Processes[i] as T;

                if (process != null)
                {
                    OnDataReadAvailable -= process.LookForData;
                    Processes.RemoveAt(i);
                }
            }
        }
        
        protected void DataReadAvailable()
        {
            OnDataReadAvailable(DataBlocks);
        }

        /// <summary>
        /// Получение первого найденного блока заданного типа.
        /// Может вернуть null.
        /// </summary>
        public T GetDataBlock<T>() where T : DataBlock
        {
            var coincidence = DataBlocks.FindType<T>();

            return coincidence;
        }

        /// <summary>
        /// Вставляет данные. Старые данные этого же типа заменются на новый блок.
        /// </summary>
        public void InsertData<TDataBlock>(TDataBlock data)
            where TDataBlock : DataBlock
        {
            var type = typeof(TDataBlock);
            
            // Если уже есть такие данные.
            if (DataBlocks.ContainsKey(type))
            {
                // Замена на новые данные.
                DataBlocks[type] = data;
                return;
            }
            
            // Добавляем ключ и данные.
            DataBlocks.Add(type,data);
        }

        public IEnumerator GetEnumeratorProcess()
        {
            return Processes.GetEnumerator();
        }

        public IEnumerator GetIenumeratorData()
        {
            return DataBlocks.GetEnumerator();
        }
    }

}
