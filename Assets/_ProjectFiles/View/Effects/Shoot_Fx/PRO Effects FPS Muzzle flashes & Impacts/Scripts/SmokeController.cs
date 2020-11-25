using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Smoke controller component.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class SmokeController : MonoBehaviour
    {
        /// <summary>
        /// Start color of particles.
        /// </summary>
        [SerializeField] [Tooltip("Start color of particles")] private Color startColor = new Color(1, 1, 1, 0);
        /// <summary>
        /// End color of particles.
        /// </summary>
        [SerializeField] [Tooltip("End color of particles")] private Color endColor = new Color(1, 1, 1, 0.3f);
        /// <summary>
        /// Start emission of particles.
        /// </summary>
        [SerializeField] [Tooltip("Start emission of particles")] private float startEmission = 0;
        /// <summary>
        /// End emission of particles.
        /// </summary>
        [SerializeField] [Tooltip("End emission of particles")] private float endEmission = 8;
        /// <summary>
        /// Start shape radius of particles emission.
        /// </summary>
        [SerializeField] [Tooltip("Start shape radius of particles emission")] private float shapeRadiusStart = 0f;
        /// <summary>
        /// End shape radius of particles emission.
        /// </summary>
        [SerializeField] [Tooltip("End shape radius of particles emission")] private float shapeRadiusEnd = 3f;
        /// <summary>
        /// Duration of animation.
        /// </summary>
        [SerializeField] [Tooltip("Duration of animation")] private float duration = 5f;

        private ParticleSystem particles;
        private float elapsedTime = 0;

        private void OnEnable()
        {
            elapsedTime = 0;
            particles = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;

            var fraction = elapsedTime / duration;

            var main = particles.main;
            var emission = particles.emission;
            var shape = particles.shape;

            var particlesStartColor = main.startColor;
            particlesStartColor.color = Color.Lerp(startColor, endColor, fraction);

            main.startColor = particlesStartColor;
            emission.rateOverTimeMultiplier = Mathf.Lerp(startEmission, endEmission, fraction);
            shape.radius = Mathf.Lerp(shapeRadiusStart, shapeRadiusEnd, fraction);
        }
    }
}