namespace Egsp.Files.Serializers
{
    /// <summary>
    /// Интерфейс для сериализаторов.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Сериализует объект.
        /// </summary>
        byte[] Serialize<T>(T obj);

        /// <summary>
        /// Десериализует объект.
        /// При неудаче возвращается default(T).
        /// </summary>
        T Deserialize<T>(byte[] serializedData);
        
        
    }
}