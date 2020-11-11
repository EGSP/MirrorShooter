using System;
using System.Collections.Generic;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.Processes
{
    public interface IProcess : IDisposable
    {
        bool IsDisposed { get; }
        
        /// <summary>
        /// Выполняет действие
        /// </summary>
        void Process();

        /// <summary>
        /// Поиск данных из списка.
        /// </summary>
        void LookForData(Dictionary<Type,DataBlock> data);
    }

    /// <summary>
    /// Процесс с определенным блоком данных
    /// </summary>
    /// <typeparam name="TDataBlock">Тип блока данных</typeparam>
    public interface IProcess<TDataBlock> : IProcess
        where TDataBlock : DataBlock
    {
        void Process(TDataBlock dataBlock);
    }
}