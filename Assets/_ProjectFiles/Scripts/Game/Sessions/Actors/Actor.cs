using System.Collections.Generic;
using Game.Entities;
using Game.EntitiesData.Team;
using Game.Net.Objects;
using Game.World;
using Gasanov.Extensions.Linq;
using UnityEngine;

namespace Game.Sessions.Actors
{
    /// <summary>
    /// Класс отвечающий за различную логику игрового процесса в сессии на стороне сервера.
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
            Session.OnUserLoaded += OnUserEnter;
            Session.OnUserDisconnected += OnUserExit;
        }

        private void OnUserEnter(UserHandler userHandler)
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
        }

        private void OnUserExit(UserHandler userHandler)
        {
            // Этим пока успешно занимается сессия.
        }

        public override void Dispose()
        {
            Session.OnUserLoaded -= OnUserEnter;
            Session.OnUserDisconnected -= OnUserExit;
        }
    }
    
    public sealed class WeaponsActor: Actor
    {
        private readonly PlayerCharactersActor charactersActor;
        
        public WeaponsActor(ServerSession session, PlayerCharactersActor charactersActor) : base(session)
        {
            this.charactersActor = charactersActor;
        }

        private void OnCharacterSpawned(PlayerEntity playerEntity)
        {
            
            
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}