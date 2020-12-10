using UnityEngine;

namespace Knife.Effects
{
    public interface ICollisionHandler
    {
        void CollisionEnter(Collision collision);
    }
}