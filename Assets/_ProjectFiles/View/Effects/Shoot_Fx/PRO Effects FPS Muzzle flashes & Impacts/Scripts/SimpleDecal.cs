using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Simple decal component.
    /// </summary>
    public class SimpleDecal : MonoBehaviour, IDecal
    {
        /// <summary>
        /// Determines decal can be rotated or not.
        /// </summary>
        [Tooltip("Determines decal can be rotated or not")] [SerializeField] private bool canRotate;

        /// <summary>
        /// Determines decal can be rotated or not.
        /// </summary>
        public bool CanRotate
        {
            get
            {
                return canRotate;
            }
        }
    }
        
    public interface IDecal
    {
        bool CanRotate { get; }
    }
}