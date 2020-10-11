﻿using System;
using Game.Chat;
using Game.Views.Chat;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Mono;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Game.Presenters.Chat
{
    public class ServerChatPresenter : SerializedMonoBehaviour, IPresenter<ServerChatView, ChatMediator>
    {
        [OdinSerialize]
        public ServerChatView View { get; private set; }
        [OdinSerialize]
        public ChatMediator Model { get; private set; }
        
        public event Action OnDispose;

        private void Awake()
        {
            Model = this.ValidateComponent(Model);
            if(Model == null)
                throw new NullReferenceException();
            
            if(View == null)
                throw new NullReferenceException();
        }
        
        private void Start()
        {
            Model.OnMessage += (x) => View.AddMessage(x);
            Model.OnShutdown += () => View.ClearChatFlow();
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