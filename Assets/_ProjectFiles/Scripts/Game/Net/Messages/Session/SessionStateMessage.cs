using Mirror;

namespace Game.Net
{
    /// <summary>
    /// Если это сообщение отправлено пользователем, значит он запрашивает информацию о сессии.
    /// Сервер отвечает сообщением этого же типа.
    /// </summary>
    public struct SessionStateMessage : NetworkMessage
    {
        /// <summary>
        /// Началась ли сессия.
        /// </summary>
        public bool IsStarted;
        
    }
}