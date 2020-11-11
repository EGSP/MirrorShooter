using System.Collections.Generic;
using Mirror;
using Sirenix.Serialization;

namespace Game.Net.Mirrors
{
    /// <summary>
    /// Добавляет только одного пользователя для объекта.
    /// </summary>
    public class OneConnectionVisibility : NetworkVisibility
    {
        public NetworkConnectionToClient SpecificConnectionToClient { get; set; }

        public override bool OnCheckObserver(NetworkConnection conn)
        {
            if (conn.connectionId == connectionToClient.connectionId)
                return true;

            return false;
        }

        public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            if (SpecificConnectionToClient != null)
            {
                observers.Add(SpecificConnectionToClient);
            } 
        }
    }
}