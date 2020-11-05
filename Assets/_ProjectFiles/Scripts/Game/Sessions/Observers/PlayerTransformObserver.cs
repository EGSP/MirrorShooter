using System.Collections.Generic;
using Game.Entities;
using Game.World;
using Gasanov.Extensions.Linq;

namespace Game.Sessions.Observers
{
    /// <summary>
    /// Следит за положением игроков на карте.
    /// Отреспавнивает их.
    /// </summary>
    public class PlayerTransformObserver : SessionObserver
    {
        /// <summary>
        /// Минимальная высота пероснажа на карте.
        /// </summary>
        private readonly float _minHeight;
        
        public PlayerTransformObserver(ServerSession serverSession) : base(serverSession)
        {
            _minHeight = -300f;
        }

        public override void Update(float deltaTime)
        {
            for (var i = 0; i < PlayerEntities.Count; i++)
            {
                var entity = PlayerEntities[i];

                if (entity.transform.position.y < _minHeight)
                {
                    entity.transform.position = SpawnPoint.SpawnPoints.Random().transform.position;
                }
            }
        }
    }
}