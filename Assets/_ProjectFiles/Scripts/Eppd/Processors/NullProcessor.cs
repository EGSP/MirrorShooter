using Gasanov.Eppd.Processors;

namespace Eppd.Processors
{
    public class NullProcessor : Processor
    {
        static NullProcessor()
        {
            Instance = new NullProcessor();
        }
        
        /// <summary>
        /// Общий экземпляр, создаваемый автоматически в static constructor.
        /// </summary>
        public static NullProcessor Instance { get; private set; }
           
        public override void Init()
        {
            return;
        }
    }
}