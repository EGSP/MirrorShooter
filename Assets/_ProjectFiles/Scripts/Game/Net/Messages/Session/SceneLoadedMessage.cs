using Mirror;

namespace Game.Net
{
    // Сообщение об окончании загрузки сцены у пользователя.
    public struct SceneLoadedMessage : NetworkMessage
    {
        public void Deserialize(NetworkReader reader)
        {
        }

        public void Serialize(NetworkWriter writer)
        {
        }
    }
}