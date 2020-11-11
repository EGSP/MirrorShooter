
using UnityEngine;

namespace Gasanov.Eppd.Processors.MonoBehaviours
{
    public abstract class ScriptableProcessor : ScriptableObject
    {
        public abstract Processor GetProcessor();
    }
}