using System.Collections.Generic;
using Mirror;

namespace Game.Net
{
    public class LobbyUsersMessage : MessageBase
    {
        public List<AddUserMessage> AddUserMessages = new List<AddUserMessage>();
    }
}