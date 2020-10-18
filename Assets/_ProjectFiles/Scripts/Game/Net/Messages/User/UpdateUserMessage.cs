using Mirror;

namespace Game.Net
{
    /// <summary>
    /// Это сообщение отправляется только с сервера, чтобы передать данные клиенту об аккаунте.
    /// </summary>
    public class UpdateUserMessage : MessageBase
    {
        public User updatedUser;
    }
}