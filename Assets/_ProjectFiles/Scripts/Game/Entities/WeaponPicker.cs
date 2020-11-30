using System.Linq;
using Game.Net.Objects;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    public class WeaponPicker : DualNetworkBehaviour
    {
        /// <summary>
        /// Разрешенные оружия.
        /// </summary>
        private SyncListString _allowedWeaponsId;

        public override void AwakeOnClient()
        {
            _allowedWeaponsId.Callback += OnAllowedWeaponsChanged;
        }

        private void OnAllowedWeaponsChanged(SyncList<string>.Operation op, int itemindex, string olditem, string newitem)
        {
            Debug.Log($"Allowed weapon {newitem}");
        }
        
        /// <summary>
        /// Очищает пердыдущий список.
        /// </summary>
        public void AllowWeapons(params string[] allowedWeaponsId)
        {
            _allowedWeaponsId.Clear();
            for (int i = 0; i < allowedWeaponsId.Length; i++)
            {
                _allowedWeaponsId.Add(allowedWeaponsId[i]);                
            }
        }
    }
}