
using UnityEngine;

namespace Gasanov.Eppd.Processes.MonoBehaviours
{
    public abstract class ScriptableProcess : ScriptableObject
    {
        public abstract IProcess GetProcess();
    }
}