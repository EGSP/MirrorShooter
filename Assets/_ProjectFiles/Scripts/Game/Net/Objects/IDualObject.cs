namespace Game.Net.Objects
{
    /// <summary>
    /// Интерфейс, который говорит о том, что объект имеет разную логику поведения на стороне сервера и клиента.
    /// </summary>
    public interface IDualObject
    {
        /// <summary>
        /// Обновление на стороне клиента.
        /// </summary>
        void UpdateOnClient();
        
        /// <summary>
        /// Обновление на стороне сервера.
        /// </summary>
        void UpdateOnServer();
    }
}