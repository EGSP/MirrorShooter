using Game.Net.Objects;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Controllers
{
    public class PlayerWeaponController : DualNetworkBehaviour
    {
        [OdinSerialize]
        public float TakeRadius { get; private set; }
        
        [OdinSerialize]
        public LayerMask Layer { get; private set; }
        
        public PlayerEntity Player { get; set; }
        
        public Weapon Current { get; private set; }

        [Server]
        private void SetWeapon(Weapon weapon)
        {
            Current = weapon;
        }

        [Command]
        public void CmdTakeWeapon()
        {
            TakeWeapon();
        }
        [Server]
        public void TakeWeapon()
        {
            if (Current != null)
            {
                Current.OnDrop();
                Current = null;
            }

            var colliders = Physics.OverlapSphere(Player.transform.position, TakeRadius, Layer);

            for (var i = 0; i < colliders.Length; i++)
            {
                var weapon = colliders[i].GetComponent<Weapon>();

                if (weapon != null)
                {
                    weapon.OnTake();
                    SetWeapon(weapon);
                    return;
                }
            }
        }
    }
}