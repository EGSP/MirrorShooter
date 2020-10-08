using Mirror;

namespace Game.Net
{
    public class AddUserMessage : MessageBase
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name;
    }
}