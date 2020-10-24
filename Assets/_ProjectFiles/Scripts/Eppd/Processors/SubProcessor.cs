using System;
using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processes;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.SubProcessors
{
    /// <summary>
    /// Является и процессором и процессом
    /// </summary>
    public abstract class SubProcessor : Processor, IProcess
    {
        public bool IsDisposed { get; protected set; }

        public void Process()
        {
            if (IsDisposed)
                return;
            
            Tick();
        }

        public abstract void LookForData(Dictionary<Type, DataBlock> data);

        public abstract void Dispose();
    }
}