using Mirror;

namespace Game.Net
{
    public class UserConnection
    {
        public enum UserSceneState
        {
            NotLoaded,
            IsLoading,
            Loaded
        }
        
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
        /// Загружен ли пользователь на текущую сцену.
        /// </summary>
        public UserSceneState SceneState;

        /// <summary>
        /// Свойство заполненности всех данных.
        /// </summary>
        public bool IsValidated
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