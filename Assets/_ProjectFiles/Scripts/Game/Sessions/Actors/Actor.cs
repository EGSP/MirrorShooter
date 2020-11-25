using System;
using System.Collections.Generic;
using Game.Entities;
using Game.EntitiesData.Team;
using Game.Net.Objects;
using Game.World;
using Gasanov.Extensions.Linq;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Sessions.Actors
{
    /// <summary>
    /// Класс отвечающий за различную логику игрового процесса в сессии на стороне сервера.
    /// Отличие от Observer в том, что Actor работает с событиями.
    /// </summary>
    public abstract class Actor
    {
        protected readonly ServerSession Session;
        
        protected Actor(ServerSession session)
        {
            Session = session;
        }

        public abstract void Dispose();
    }

    public sealed class PlayerCharactersActor : Actor
    {
        public PlayerCharactersActor(ServerSession session) : base(session)
        {
            Session.OnUserLoaded += SpawnPlayerCharacter;
            Session.OnUserDisconnected += DestroyPlayerCharacter;
        }
        
        public event Action<PlayerEntity> OnPlayerEntitySpawned=delegate(PlayerEntity entity) {  };

        private void SpawnPlayerCharacter(UserHandler userHandler)
        {
            // Спавним игрока
            var playerEntity = Object.Instantiate(Session.PlayerEntityPrefab);
            playerEntity.gameObject.transform.position = SpawnPoint.SpawnPoints.Random().transform.position;
            playerEntity.owner = userHandler.UserConnection.User;
            playerEntity.team = TeamType.Chaos;
            NetworkFactory.SpawnForAll(playerEntity.gameObject, userHandler.UserConnection);
            
            // Спавн контроллера
            var playerController =  Object.Instantiate(Session.PlayerController);
            playerController.gameObject.name = $"PC [{playerEntity.owner.id}]";
            NetworkFactory.SpawnForConnection(playerController.gameObject, userHandler.UserConnection);
            playerController.SetPlayerEntity(playerEntity);
            playerController.playerEntityId = playerEntity.netId;
            
            userHandler.RelatedPlayerEntity = playerEntity;
            userHandler.AddGameObject(playerEntity.gameObject);
            userHandler.AddGameObject(playerController.gameObject);
            
            Session.AddPlayerEntity(playerEntity);
            OnPlayerEntitySpawned(playerEntity);
        }

        private void DestroyPlayerCharacter(UserHandler userHandler)
        {
            // Этим пока успешно занимается сессия.
        }

        public override void Dispose()
        {
            Session.OnUserLoaded -= SpawnPlayerCharacter;
            Session.OnUserDisconnected -= DestroyPlayerCharacter;
        }
    }
    
    public sealed class WeaponsActor: Actor
    {
        private readonly PlayerCharactersActor charactersActor;

        private readonly WeaponPicker weaponPickerPrefab;

        private readonly Dictionary<int, WeaponPicker> weaponPickers;
        
        public WeaponsActor(ServerSession session, PlayerCharactersActor charactersActor) : base(session)
        { 
            this.charactersActor = charactersActor;
            
            weaponPickers = new Dictionary<int, WeaponPicker>();
            weaponPickerPrefab = Resources.Load<WeaponPicker>("Prefabs/Weapons/weapon_picker");

            charactersActor.OnPlayerEntitySpawned += AllowWeaponForCharacter;
        }

        private void SpawnWeaponPicker(PlayerEntity playerEntity)
        {
            UserHandler userHandler;
            if (Session.GetUserHandler(playerEntity, out userHandler))
            {
                var inst = Object.Instantiate(weaponPickerPrefab);

                weaponPickers.Add(userHandler.Id, inst);
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
            SpawnWeaponPicker(playerEntity);
            
            var inst = weaponPickers[playerEntity.owner.id];
            
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

        public override void Dispose()
        {
            charactersActor.OnPlayerEntitySpawned -= AllowWeaponForCharacter;
        }
    }
}