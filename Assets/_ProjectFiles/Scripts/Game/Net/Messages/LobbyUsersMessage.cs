using System.Collections.Generic;
using Mirror;

namespace Game.Net
{
    public struct LobbyUsersMessage : NetworkMessage
    {
        public List<AddUserMessage> AddUserMessages;
    }
}