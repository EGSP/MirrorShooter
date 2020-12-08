using Mirror;

namespace Game.Net
{
    public struct DisconnectUserMessage : NetworkMessage
    {
        public User user;
    }
}