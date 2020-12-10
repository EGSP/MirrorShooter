using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Flashbang blind postprocess component.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FlashbangPostprocess : MonoBehaviour, IBlinder
    {
        /// <summary>
        /// Total blind duration.
        /// </summary>
        [SerializeField] [Tooltip("Total blind duration")] private float blindDuration = 5f;
        /// <summary>
        /// White screen blending curve.
        /// </summary>
        [SerializeField] [Tooltip("White screen blending curve")] private AnimationCurve whiteScreenCurve;
        /// <summary>
        /// Last frame blending curve.
        /// </summary>
        [SerializeField] [Tooltip("Last frame blending curve")] private AnimationCurve lastFrameCurve;
        /// <summary>
        /// Post process material.
        /// </summary>
        [SerializeField] [Tooltip("Post process material")] private Material material;
        /// <summary>
        /// Blending amount curve by distance.
        /// </summary>
        [SerializeField] [Tooltip("Blending amount curve by distance")] private AnimationCurve distanceAmountCurve;
        /// <summary>
        /// Max distance to blind.
        /// </summary>
        [SerializeField] [Tooltip("Max distance to blind")] private float maxDistance = 20f;
        /// <summary>
        /// Blending amount curve by angle (dot result values [-1;1]).
        /// </summary>
        [SerializeField] [Tooltip("Blending amount curve by angle (dot result values [-1;1])")] private AnimationCurve angleAmountCurve;

        private RenderTexture lastFrame;
        private bool isBlinded = false;
        private float blindTime;
        private float blindAmount;

        private bool updateLastFrame;

        private Camera attachedCamera;

        /// <summary>
        /// IBlinder.Blind implementation. Blinds attached camera.
        /// </summary>
        /// <param name="amount">amount multiplier</param>
        /// <param name="position">blind source position</param>
        public void Blind(float amount, Vector3 position)
        {
            Vector3 direction = position - transform.position;

            float distanceFraction = direction.magnitude / maxDistance;
            float dotResult = Vector3.Dot(transform.forward, direction.normalized);

            amount = distanceAmountCurve.Evaluate(distanceFraction) * angleAmountCurve.Evaluate(dotResult);

            amount = Mathf.Clamp01(amount);

            isBlinded = true;
            blindTime = 0;
            blindAmount = amount;
            updateLastFrame = true;
        }

        private void OnEnable()
        {
            if(attachedCamera == null)
                attachedCamera = GetComponent<Camera>();

            lastFrame = RenderTexture.GetTemporary(attachedCamera.pixelWidth, attachedCamera.pixelHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        }

        private void OnDisable()
        {
            RenderTexture.ReleaseTemporary(lastFrame);
        }

        private void Update()
        {
            if (isBlinded)
            {
                blindTime += Time.deltaTime;
                if (blindTime >= blindDuration)
                    isBlinded = false;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (isBlinded)
            {
                if (updateLastFrame)
                {
                    updateLastFrame = false;
                    Graphics.Blit(source, lastFrame);
                }

                float fraction = blindTime / blindDuration;

                material.SetFloat("_White", whiteScreenCurve.Evaluate(fraction) * blindAmount);
                material.SetFloat("_Last", lastFrameCurve.Evaluate(fraction) * blindAmount);
                material.SetTexture("_BlendTex", lastFrame);
                Graphics.Blit(source, destination, material, 0);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}