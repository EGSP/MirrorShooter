using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Component to destroy object with delay from OnEnable event.
    /// </summary>
    public class AutoDestroyer : MonoBehaviour
    {
        /// <summary>
        /// Destroy delay in seconds.
        /// </summary>
        [SerializeField] [Tooltip("Destroy delay in seconds")] private float destroyDelay = 20f;

        /// <summary>
        /// OnEnable called by Unity automatically.
        /// </summary>
        private void OnEnable()
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}