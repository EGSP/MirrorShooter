using System.Collections.Generic;
using System.Linq;
using Game.Entities;

namespace Game.Sessions.Observers
{
    /// <summary>
    /// Следит за объектами в сессии.
    /// </summary>
    public abstract class SessionObserver
    {
        protected readonly ServerSession ServerSession;
        
        protected readonly List<PlayerEntity> PlayerEntities;
        
        protected SessionObserver(ServerSession serverSession)
        {
            ServerSession = serverSession;
            PlayerEntities = new List<PlayerEntity>();
            ServerSession.OnPlayerEntityAdd += AddPlayerEntity;
            ServerSession.OnPlayerEntityRemove += RemovePlayerEntity;
        }

        private void AddPlayerEntity(PlayerEntity playerEntity)
        {
            var coincidence = PlayerEntities.FirstOrDefault(x=> x== playerEntity);
            
            // Он не был зарегестрирован.
            if (coincidence == null)
            {
                PlayerEntities.Add(playerEntity);
            }
        }

        private void RemovePlayerEntity(PlayerEntity playerEntity)
        {
            var coincidence = PlayerEntities.FirstOrDefault(x=> x== playerEntity);

            // Он был зарегестрирован.
            if (coincidence != null)
            {
                PlayerEntities.Remove(playerEntity);
            }
        }
        
        public virtual void Update(float deltaTime)
        {
        }
    }
}