using System.Collections;
using System.Collections.Generic;
using Game.Net;
using UnityEngine;

namespace Game.Configuration
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

        public static LaunchModeType LaunchMode;
    }
}
