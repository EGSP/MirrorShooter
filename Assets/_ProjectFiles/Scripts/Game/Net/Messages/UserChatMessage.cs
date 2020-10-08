using Mirror;

namespace Game.Net
{
    public class UserChatMessage : MessageBase
    {
        /// <summary>
        /// От какого пользователя сообщение.
        /// </summary>
        public string From;
        
        /// <summary>
        /// Текст сообщения.
        /// </summary>
        public string Text;
    }
}