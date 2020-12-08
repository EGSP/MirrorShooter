using Sirenix.Serialization;

namespace Egsp.Files.Serializers
{
    public sealed class OdinSerializer : ISerializer
    {
        /// <summary>
        /// Сериализует объект в бинарном формате.
        /// </summary>
        public byte[] Serialize<T>(T obj)
        {
            var serializationData = SerializationUtility.SerializeValue<T>(obj, DataFormat.JSON);
            return serializationData;
        }

        public T Deserialize<T>(byte[] serializedData)
        {
            var obj = SerializationUtility.DeserializeValue<T>(serializedData, DataFormat.JSON);
            return obj;
        }
    }
}