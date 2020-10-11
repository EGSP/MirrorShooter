using System;
using Game.Net;
using Mirror;
using UnityEngine;

namespace Game.Chat
{
    public class ChatMediator : MonoBehaviour
    {
        /// <summary>
        /// Вызывается при получении сообщения.
        /// </summary>
        public event Action<UserChatMessage> OnMessage = delegate(UserChatMessage message) {  };
        
        public event Action OnShutdown = delegate {  };

        private ServerLobby _serverLobby;
        
        public void Awake()
        {
            _serverLobby = ServerLobby.Instance;

            // Регестрируем сообщения при старте сервера.
            _serverLobby.OnStarted += () =>
            {
                NetworkServer.RegisterHandler<UserChatMessage>(OnUserMessage);
            };

            _serverLobby.OnShutdown += () => OnShutdown();
        }

        /// <summary>
        /// Обработка входящего сообщения.
        /// Полученное сообщение рассылается остальным пользователям.
        /// </summary>
        private void OnUserMessage(UserChatMessage message)
        {
            OnMessage(message);
            
            _serverLobby.SendToAll<UserChatMessage>(message, x=>x.Name == message.From);
        }
    }
}