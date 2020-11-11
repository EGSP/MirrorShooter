using Gasanov.Eppd.Processes;

namespace Gasanov.Eppd.Processors.Mediators
{
    public abstract class ProcessMediator
    {
        /// <summary>
        /// Отправляет сообщение другим процессам
        /// </summary>
        /// <param name="process">Процесс отправитель</param>
        public abstract void Send(IProcess process);
    }
}