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
        [SyncVar(hook = nameof(OnAllowedWeaponsChanged))] 
        private SyncListString _allowedWeaponsId;
 
        
        private void OnAllowedWeaponsChanged(string[] oldIds, string[] newIds)
        {
            Debug.Log(string.Join("; ", newIds));
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