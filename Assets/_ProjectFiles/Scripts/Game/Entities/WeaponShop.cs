using System;
using System.Collections.Generic;
using System.Linq;
using Game.EntitiesData.Team;
using Game.Net.Objects;
using Game.Net.Resources;
using Mirror;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.Entities
{
    // Этот объект должен спавниться только у конкретного игрока. На сервере их столько же сколько и игроков.
    public class WeaponShop : DualNetworkBehaviour
    {
        private static Dictionary<string, Weapon> _weaponsPrefabs = new Dictionary<string, Weapon>();
        private static string _prefabsPath = "Prefabs/Weapons/";
        
        public event Action OnEquipAllowed = delegate {  };
        
        public bool EquipAllowed { get; private set; }
        
        /// <summary>
        /// Разрешенные оружия.
        /// </summary>
        private SyncListString _allowedWeaponsId;
        
        
        
        

        public override void AwakeOnClient()
        {
            LoadClientResources();
            _allowedWeaponsId.Callback += OnAllowedWeaponsChanged;
        }

        [Client]
        private void OnAllowedWeaponsChanged(SyncList<string>.Operation op, int itemindex, string olditem, string newitem)
        {
            Debug.Log($"Allowed weapon {newitem}");
        }

        [Server]
        private void SpawnWeapon(string weaponName)
        {
            if (_weaponsPrefabs.ContainsKey(weaponName))
            {
                var inst = Instantiate(_weaponsPrefabs[weaponName]);
                NetworkFactory.SpawnForAll(inst.gameObject);
            }
        }

        [Server]
        private void OnWeaponDroped(Weapon weapon)
        {
            NetworkFactory.Destroy(weapon.gameObject);
        }

        [Server]
        private void OnWeaponTaked(Weapon weapon)
        {
            
        }

        [TargetRpc]
        private void TargetAllowEquip()
        {
            EquipAllowed = true;
            OnEquipAllowed();
        }
        [Server]
        private void AllowEquip()
        {
            EquipAllowed = true;
            OnEquipAllowed();
            TargetAllowEquip();
        }
        
        /// <summary>
        /// Очищает предыдущий список при вызове.
        /// </summary>
        [Server]
        public void AllowWeapons(params string[] allowedWeaponsId)
        {
            _allowedWeaponsId.Clear();
            for (int i = 0; i < allowedWeaponsId.Length; i++)
            {
                _allowedWeaponsId.Add(allowedWeaponsId[i]);                
            }
        }

        [Server]
        public static void LoadWeaponsPrefabs(params string[] weapons)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                var name = weapons[i];
                if (!_weaponsPrefabs.ContainsKey(name))
                {
                    if(name.IsNullOrWhitespace())
                        continue;
                    
                    var prefab = Resources.Load<Weapon>(_prefabsPath + weapons[i]);

                    if (prefab != null)
                    {
                           _weaponsPrefabs.Add(name,prefab);
                    }
                    else
                    {
                        Debug.LogWarning($"Оружие {name} по пути {_prefabsPath} не найдено в ресурсах игры.");
                    }
                }   
            }
        }

        [Client]
        private static void LoadClientResources()
        {
            NetworkResources.LoadAndRegister(new WeaponResourceLoader(
                TeamWeapons(TeamType.Chaos).Concat(TeamWeapons(TeamType.Rondo)).ToArray(), _prefabsPath));
        }
        
        
        public static string[] TeamWeapons(TeamType team)
        {
            if (team == TeamType.Chaos)
            {
                return new string[]{ "kalak", "pratol", "brada"};
            }
            else
            {
                return new string[]{"matir", "erew", "looperk"}; 
            }
            
            throw new Exception("Invalid team");
        }
        
        
    }

    public sealed class WeaponResourceLoader : IResourceLoader
    {
        private readonly string[] weapons;
        private readonly string path;

        public WeaponResourceLoader(string[] weapons, string path)
        {
            this.weapons = weapons;
            this.path = path;
        }
        
        public List<GameObject> LoadPrefabs()
        {
            var list = new List<GameObject>();

            for (int i = 0; i < weapons.Length; i++)
            {
                var name = weapons[i];
                
                if(name.IsNullOrWhitespace())
                    continue;
                    
                var prefab = Resources.Load<Weapon>(path + weapons[i]);

                if (prefab != null)
                {
                    list.Add(prefab.gameObject);
                }
                else
                {
                    Debug.LogWarning($"Оружие {name} по пути {path} не найдено в ресурсах игры.");
                }
            }
            
            return list;
        }
    }
}