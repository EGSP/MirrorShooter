using System;
using Game.Net;
using Mirror;
using UnityEngine;

namespace Game.Chat
{
    public class ChatClient : MonoBehaviour
    {
        public event Action<UserChatMessage> OnMessage = delegate(UserChatMessage message) {  };
        
        private ClientLobby _clientLobby;
        
        public event Action OnDisconnect = delegate {  };
        
        private void Awake()
        {
            _clientLobby = ClientLobby.Instance;

            RegisterHandlers();
            // В первый раз он уже может быть соединенным.
            _clientLobby.OnConnected += RegisterHandlers;
            _clientLobby.OnDisconnect += Disconnect;
        }

        private void OnDisable()
        {
            _clientLobby.OnConnected -= RegisterHandlers;
            _clientLobby.OnDisconnect -= Disconnect;
        }

        private void Disconnect()
        {
            OnDisconnect();
        }

        private void RegisterHandlers()
        {
            NetworkClient.RegisterHandler<UserChatMessage>(OnUserMessage);
        }

        private void OnUserMessage(UserChatMessage message)
        {
            OnMessage(message);
        }

        /// <summary>
        /// Отправка сообщения.
        /// </summary>
        public void SendMessageToServer(string message)
        {
            var mes = new UserChatMessage();
            mes.From = _clientLobby.MainUser.name;
            mes.Text = message;

            // Отправляем самому себе.
            OnMessage(mes);

            NetworkClient.Send<UserChatMessage>(mes);
        }
    }
}