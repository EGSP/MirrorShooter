using Mirror;

namespace Game.Net
{
    public class DisconnectUserMessage : MessageBase
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name;
    }
}