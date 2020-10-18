using System;

namespace Game.Net
{
    [Serializable]
    public sealed class User
    {
        /// <summary>
        /// Используется только для Weaver
        /// </summary>
        public User()
        {
            
        }
        
        public User(string name)
        {
            this.name = name;
            id = -1;
        }

        public string name;

        /// <summary>
        /// Уникальный идентификатор.
        /// </summary>
        public int id;
        
        /// <summary>
        /// Готов ли пользователь ко входу в игру.
        /// </summary>
        public bool IsReady { get; set; }
    }
}