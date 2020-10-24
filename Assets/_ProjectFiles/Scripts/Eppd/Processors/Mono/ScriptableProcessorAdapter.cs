using UnityEngine;

namespace Gasanov.Eppd.Processors.MonoBehaviours
{
    [RequireComponent(typeof(ProcessorAdapter))]
    public class ScriptableProcessorAdapter : MonoBehaviour
    {
        // Адаптируемый процесс в виде скриптового объекта
        public ScriptableProcessor adapteeScriptableProcessor;

        private void Awake()
        {
            var procAdapter = GetComponent<ProcessorAdapter>();
            procAdapter.AdapteeProcessor = adapteeScriptableProcessor.GetProcessor();
        }
    }
}