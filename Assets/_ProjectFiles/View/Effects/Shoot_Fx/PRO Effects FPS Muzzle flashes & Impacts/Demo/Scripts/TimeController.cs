using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Time controller component.
    /// </summary>
    public class TimeController : MonoBehaviour
    {
        /// <summary>
        /// Target timeScale.
        /// </summary>
        [SerializeField] [Tooltip("Target timeScale")] private float targetScale = 0.1f;
        /// <summary>
        /// Timescale blending speed.
        /// </summary>
        [SerializeField] [Tooltip("Timescale blending speed")] private float blendSpeed = 2f;

        private bool isActivated = false;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                isActivated = !isActivated;
            }

            Time.timeScale = Mathf.MoveTowards(Time.timeScale, isActivated ? targetScale : 1, Time.unscaledDeltaTime * blendSpeed);
        }
    }
}