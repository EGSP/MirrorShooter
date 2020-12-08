using Mirror;

namespace Game.Net
{
    public struct AddUserMessage : NetworkMessage
    {
        public User user;
    }
}