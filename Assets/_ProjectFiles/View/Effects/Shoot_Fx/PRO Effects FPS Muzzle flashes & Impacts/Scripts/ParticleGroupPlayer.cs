using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Particle group player component
    /// </summary>
    public class ParticleGroupPlayer : MonoBehaviour
    {
        /// <summary>
        /// Particle systems array.
        /// </summary>
        [SerializeField] [Tooltip("Particle systems array")] private ParticleSystem[] particleSystems;

        /// <summary>
        /// Plays particle systems from particleSystems array.
        /// </summary>
        public void Play()
        {
            foreach (var ps in particleSystems)
            {
                ps.Play();
            }
        }

        /// <summary>
        /// Stops particle systems from particleSystems array.
        /// </summary>
        public void Stop()
        {
            foreach (var ps in particleSystems)
            {
                ps.Clear();
                ps.Stop();
            }
        }
    }
}