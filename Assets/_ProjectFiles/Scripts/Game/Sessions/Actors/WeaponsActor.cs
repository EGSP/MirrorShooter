using System;
using System.Collections.Generic;
using Game.Entities;
using Game.EntitiesData.Team;
using Game.Net.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Sessions.Actors
{
    public sealed class WeaponsActor: Actor
    {
        private readonly PlayerCharactersActor charactersActor;

        private readonly WeaponShop weaponShopPrefab;

        private readonly Dictionary<int, WeaponShop> weaponShops;
        
        public WeaponsActor(ServerSession session, PlayerCharactersActor charactersActor) : base(session)
        { 
            this.charactersActor = charactersActor;
            
            weaponShops = new Dictionary<int, WeaponShop>();
            weaponShopPrefab = Resources.Load<WeaponShop>("Prefabs/Weapons/weapon_picker");

            charactersActor.OnPlayerEntitySpawned += AllowWeaponForCharacter;
            charactersActor.OnPlayerEntityDestroy += DestroyWeaponShop;
            
            LoadResources();
        }

        private void LoadResources()
        {
            WeaponShop.LoadWeaponsPrefabs(WeaponShop.TeamWeapons(TeamType.Chaos));
            WeaponShop.LoadWeaponsPrefabs(WeaponShop.TeamWeapons(TeamType.Rondo));
        }

        private void SpawnWeaponShop(PlayerEntity playerEntity)
        {
            UserHandler userHandler;
            if (Session.GetUserHandler(playerEntity, out userHandler))
            {
                var inst = Object.Instantiate(weaponShopPrefab);

                weaponShops.Add(userHandler.Id, inst);
                NetworkFactory.SpawnForConnection(inst.gameObject, userHandler.UserConnection);
            }
            else
            {
                Debug.Log($"У сущности netId{playerEntity.netId} : id {playerEntity.owner.id} - не имеется " +
                          $"UserHandler");
            }
        }

        private void AllowWeaponForCharacter(PlayerEntity playerEntity)
        {
            SpawnWeaponShop(playerEntity);
            
            var inst = weaponShops[playerEntity.owner.id];
            
            if (playerEntity.team == TeamType.Chaos)
            {
                inst.AllowWeapons("kalak", "pratol", "brada");
                return;
            }
            else
            {
                inst.AllowWeapons("matir", "erew", "looperk");    
            }
        }

        private void DestroyWeaponShop(PlayerEntity playerEntity)
        {
            UserHandler userHandler;
            if (Session.GetUserHandler(playerEntity, out userHandler))
            {
                if (weaponShops.ContainsKey(userHandler.Id))
                {
                    var weaponShop = weaponShops[userHandler.Id];
                    
                    NetworkFactory.Destroy(weaponShop.gameObject);
                }
            }
        }

        public override void Dispose()
        {
            charactersActor.OnPlayerEntitySpawned -= AllowWeaponForCharacter;
            charactersActor.OnPlayerEntityDestroy -= DestroyWeaponShop;
        }
    }
}