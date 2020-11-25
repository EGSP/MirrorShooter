using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.Effects
{
    /// <summary>
    /// Component that helps to place object on collision point or by raycast to ground.
    /// </summary>
    public class CollisionPlacer : MonoBehaviour, ICollisionHandler, IAutoPlacer
    {
        /// <summary>
        /// Raycast mask.
        /// </summary>
        [SerializeField] [Tooltip("Raycast mask")] private LayerMask mask = ~0;
        /// <summary>
        /// Raycast max distance.
        /// </summary>
        [SerializeField] [Tooltip("Raycast max distance")] private float autoPlaceMaxDistance = 6f;

        /// <summary>
        /// ICollisionHandler.CollisionEnter implementation. Places at average point from collision contacts.
        /// </summary>
        /// <param name="collision"></param>
        public void CollisionEnter(Collision collision)
        {
            Vector3 point = Vector3.zero;
            Vector3 normal = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                var c = collision.contacts[i];
                point += c.point;
                normal += c.normal;
            }

            point /= collision.contactCount;
            normal /= collision.contactCount;
            normal.Normalize();

            transform.position = point + normal * 0.01f;
            transform.rotation = Quaternion.AngleAxis(Random.value * 360, normal) * Quaternion.LookRotation(-normal);
        }

        /// <summary>
        /// IAutoPlacer.AutoPlace implementation. Places at raycast hit point with direction Vector3.down.
        /// </summary>
        public void AutoPlace()
        {
            Ray r = new Ray(transform.position, Vector3.down);
            RaycastHit hitInfo;

            if(Physics.Raycast(r, out hitInfo, autoPlaceMaxDistance, mask))
            {
                var point = hitInfo.point;
                var normal = hitInfo.normal;
                transform.position = point + normal * 0.01f;
                transform.rotation = Quaternion.AngleAxis(Random.value * 360, normal) * Quaternion.LookRotation(-normal);
            }
        }
    }
}