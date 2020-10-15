using Mirror;

namespace Game.Net
{
    public class UserChatMessage : MessageBase
    {
        public User From;

        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text;
    }
}