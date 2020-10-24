using System;
using System.Collections.Generic;
using System.Linq;
using Game.Processors;
using Gasanov.Eppd.Data;
using Gasanov.Eppd.Processors;

namespace Gasanov.Eppd.Processes
{
    public abstract class ProcessBase : IProcess
    {
        public bool IsDisposed { get; set; }

        public virtual void Dispose()
        {
            IsDisposed = true;
        }

        public abstract void Process();

        public abstract void LookForData(Dictionary<Type, DataBlock> data);

        /// <summary>
        /// Ищет блок данных в словаре, если текущее значение равно null.
        /// </summary>
        protected void TryGetData<TDataBlock>(Dictionary<Type, DataBlock> data, ref TDataBlock dataBlock)
            where TDataBlock : DataBlock
        {
            if (dataBlock == null)
            {
                var movePair = data.FirstOrDefault(x =>
                    (x.Value as TDataBlock) != null);

                if (movePair.Value != null)
                {
                    dataBlock = movePair.Value as TDataBlock;
                }
            }
        }

        /// <summary>
        /// Возвращает true, если любой из блоков данных null или IsDisposed
        /// </summary>
        protected bool DataNullOrDisposed(params DataBlock[] dataBlocks)
        {
            for (var i = 0; i < dataBlocks.Length; i++)
            {
                var datablock = dataBlocks[i];
                
                if (datablock == null)
                    return true;

                if (datablock.IsDisposed)
                    return true;
            }

            return false;
        }
    }
}