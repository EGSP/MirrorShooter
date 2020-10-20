using Mirror;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerController : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnEntityChanged))]
        public uint playerEntityId;

        private PlayerEntity _playerEntity;

        private void OnEntityChanged(uint _, uint newEntity)
        {
            if (NetworkIdentity.spawned.TryGetValue(newEntity, out NetworkIdentity identity))
            {
                _playerEntity = identity.GetComponent<PlayerEntity>();
                
                _playerEntity.SetCamera(Camera.main);
            }
            else
            {
                Debug.Log("PlayerEntity не найден.");
            }
        }
    }
}