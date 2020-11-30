using UnityEngine;

namespace Knife.Effects
{
    public interface IBlinder
    {
        void Blind(float amount, Vector3 point);
    }
}