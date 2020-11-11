using System;
using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;
using Gasanov.Extensions.Linq;

namespace Gasanov.Eppd.Processes
{
    /// <summary>
    /// Группирует процессы в один процесс. Альтернатива SubProcessor
    /// </summary>
    public class GroupProcess : IProcess
    {
        private readonly IProcess[] _processes;

        public GroupProcess(params IProcess[] processes)
        {
            _processes = processes;
        }

        public GroupProcess(List<IProcess> processes) : this (processes.ToArray())
        {
        }
        
        public bool IsDisposed { get; set; }
        
        public void Process()
        {
            if (IsDisposed)
                return;
            
            for (var i = 0; i < _processes.Length; i++)
            {
                _processes[i].Process();
            }
        }

        public void LookForData(Dictionary<Type, DataBlock> data)
        {
            _processes.ForEach(x => x.LookForData(data));
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}