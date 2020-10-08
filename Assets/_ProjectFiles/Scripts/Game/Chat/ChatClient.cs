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
        
        private void Awake()
        {
            _clientLobby = ClientLobby.Instance;

            _clientLobby.OnConnected += () =>
            {
                NetworkClient.RegisterHandler<UserChatMessage>(OnUserMessage);
            };
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
            mes.From = _clientLobby.MainUser.Name;
            mes.Text = message;

            // Отправляем самому себе.
            OnMessage(mes);

            NetworkClient.Send<UserChatMessage>(mes);
        }
    }
}