using System;
using Game.Chat;
using Game.Net;
using Game.Views.Chat;
using Gasanov.Core.Mvp;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Game.Presenters.Chat
{
    public class ClientChatPresenter: SerializedMonoBehaviour, IPresenter<ClientChatView, ChatClient>
    {
        [OdinSerialize]
        public ClientChatView View { get; private set; }
        [OdinSerialize]
        public ChatClient Model { get; private set; }
        
        public event Action OnDispose;
        
        
        private void OnEnable()
        {
            Model.OnMessage += AddMessage;
            Model.OnDisconnect += ClearChat;
            View.OnMessageSend += SendMessage;
        }

        private void OnDisable()
        {
            Model.OnMessage -= AddMessage;
            Model.OnDisconnect -= ClearChat;
            View.OnMessageSend -= SendMessage;
        }

        private void AddMessage(UserChatMessage message)
        {
            View.AddMessage(message);
        }

        private void ClearChat()
        {
            View.ClearChatFlow();
        }

        private void SendMessage(string message)
        {
            Model.SendMessageToServer(message);
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Share()
        {
            throw new NotImplementedException();
        }

        public bool Response(string key, object arg)
        {
            throw new NotImplementedException();
        }
    }
}