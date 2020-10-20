using Game.Net;
using Mirror;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerEntity : NetworkBehaviour
    {
        [SyncVar]
        public User owner;
    }
}