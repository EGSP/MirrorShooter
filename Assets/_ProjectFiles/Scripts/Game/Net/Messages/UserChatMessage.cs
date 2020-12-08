using Mirror;

namespace Game.Net
{
    public struct UserChatMessage : NetworkMessage
    {
        public User From;

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text;
    }
}