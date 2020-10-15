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
        public event Action OnStart = delegate {  };

        private ServerLobby _serverLobby;
        
        public void Awake()
        {
            _serverLobby = ServerLobby.Instance;

            RegisterHandlers();
            // Регестрируем сообщения при старте сервера.
            _serverLobby.OnStarted += RegisterHandlers;

            _serverLobby.OnShutdown += Shutdown;
            _serverLobby.OnStarted += ChatStart;
        }

        private void OnDestroy()
        {
            _serverLobby.OnStarted -= RegisterHandlers;
            _serverLobby.OnShutdown -= Shutdown;
            _serverLobby.OnStarted -= ChatStart;
        }

        private void RegisterHandlers()
        {
            NetworkServer.RegisterHandler<UserChatMessage>(OnUserMessage);
        }

        /// <summary>
        /// Обработка входящего сообщения.
        /// Полученное сообщение рассылается остальным пользователям.
        /// </summary>
        private void OnUserMessage(UserChatMessage message)
        {
            OnMessage(message);
            
            _serverLobby.SendToAll<UserChatMessage>(message, x=>x.id == message.From.id);
        }

        private void Shutdown()
        {
            OnShutdown();
        }

        private void ChatStart()
        {
            OnStart();
        }
    }
}