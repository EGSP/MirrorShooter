using System;
using Game.Entities;
using Game.EntitiesData.Team;
using Game.Net.Objects;
using Game.World;
using Gasanov.Extensions.Linq;
using Object = UnityEngine.Object;

namespace Game.Sessions.Actors
{
    public sealed class PlayerCharactersActor : Actor
    {
        public PlayerCharactersActor(ServerSession session) : base(session)
        {
            Session.OnUserLoaded += SpawnPlayerCharacter;
            Session.OnUserDisconnected += DestroyPlayerCharacter;
        }
        
        public event Action<PlayerEntity> OnPlayerEntitySpawned = delegate(PlayerEntity entity) {  };
        public event Action<PlayerEntity> OnPlayerEntityDestroy = delegate(PlayerEntity entity) {  };

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
            if (userHandler.RelatedPlayerEntity != null)
                OnPlayerEntityDestroy(userHandler.RelatedPlayerEntity);

            // Уничтожением пока успешно занимается сессия.
        }

        public override void Dispose()
        {
            Session.OnUserLoaded -= SpawnPlayerCharacter;
            Session.OnUserDisconnected -= DestroyPlayerCharacter;
        }
    }
}