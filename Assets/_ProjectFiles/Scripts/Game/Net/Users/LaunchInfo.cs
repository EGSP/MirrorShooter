using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Net
{
    /// <summary>
    /// Хранит всю информацию о запуске игры.
    /// </summary>
    public static class LaunchInfo
    {
        /// <summary>
        /// Текущий пользователь игры.
        /// При запуске сервера ссылки нет.
        /// </summary>
        public static User User;
    }
}
