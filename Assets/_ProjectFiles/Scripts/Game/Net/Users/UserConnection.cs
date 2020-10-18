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

        /// <summary>
        /// Свойство заполненности всех данных.
        /// </summary>
        public bool FullVal
        {
            get
            {
                if (Connection == null)
                    return false;

                if (User == null)
                    return false;

                if (User.id == -1)
                    return false;

                return true;
            }
        }
    }
}