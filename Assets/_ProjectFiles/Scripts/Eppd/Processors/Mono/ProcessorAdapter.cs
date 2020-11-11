
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gasanov.Eppd.Processors.MonoBehaviours
{
    /// <summary>
    /// Адаптирует процессоры под MonoBehaviour
    /// </summary>
    public class ProcessorAdapter : SerializedMonoBehaviour
    {
        // Адаптируемый процесс
        [OdinSerialize]
        public Processor AdapteeProcessor { get; set; }

        protected virtual void Start()
        {
            AdapteeProcessor.Init();    
        }
        
        protected virtual void Update()
        {
            AdapteeProcessor.Tick();
        }
    }
}