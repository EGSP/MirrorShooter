using Mirror;

namespace Game.Net
{
    public class UserConnection
    {
        public UserConnection(NetworkConnection conn)
        {
            Connection = conn;
        }
        
        /// <summary>
        /// Соединение связанное с пользователем.
        /// </summary>
        public NetworkConnection Connection;

        /// <summary>
        /// Пользователь связанный с соединением.
        /// </summary>
        public User User;
    }
}