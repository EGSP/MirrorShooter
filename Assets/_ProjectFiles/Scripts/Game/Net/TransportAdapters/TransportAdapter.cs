using UnityEngine;

namespace Game.Net
{
    public abstract class TransportAdapter : MonoBehaviour
    {
        /// <summary>
        /// Устанавливает порт.
        /// </summary>
        /// <param name="port"></param>
        public abstract void SetPort(string port);
    }
}