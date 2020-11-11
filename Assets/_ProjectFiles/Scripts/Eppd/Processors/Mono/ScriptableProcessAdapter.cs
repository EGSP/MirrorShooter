using System.Collections.Generic;
using Gasanov.Eppd.Processors.MonoBehaviours;
using UnityEngine;

namespace Gasanov.Eppd.Processes.MonoBehaviours
{
    [RequireComponent(typeof(ProcessorAdapter))]
    public class ScriptableProcessAdapter : MonoBehaviour
    {
        // Адаптируемые процессы
        public List<ScriptableProcess> adapteeScriptableProcesses;

        private void Awake()
        {
            var procAdapter = GetComponent<ProcessorAdapter>();

            for (int i = 0; i < adapteeScriptableProcesses.Count; i++)
            {
                var process = adapteeScriptableProcesses[i].GetProcess();
                procAdapter.AdapteeProcessor.AppendProcess(process);
            }
        }
    }
}