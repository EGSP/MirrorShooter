using System;
using Game.Net;
using Gasanov.Core.Mvp;
using Gasanov.Utils.GameObjectUtilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Chat
{
    public class ClientChatView : MonoBehaviour, IView
    {
        
        [SerializeField] private GameObject chatFlow;
        
        [SerializeField] private TMP_InputField messageInput;
        [SerializeField] private int messageLength;
        
        [SerializeField] private int maxTextboxes;
        [SerializeField] private TMP_Text textboxPrefab;
        

        public event Action<string> OnMessageSend = delegate(string s) { };

        private void Awake()
        {
            messageInput.characterLimit = messageLength;
        }

        public void AddMessage(UserChatMessage message)
        {
            var inst = Instantiate(textboxPrefab, chatFlow.transform, false);
            inst.text = $"{message.From}: {message.Text}";

            // Удаление старых сообщений.
            if (chatFlow.transform.childCount > maxTextboxes)
            {
                Destroy(chatFlow.transform.GetChild(0).gameObject);
            }
        }
        
        public void SendMessageToServer()
        {
            if (messageInput.text.IsNullOrWhitespace())
                return;

            OnMessageSend(messageInput.text);
            messageInput.text = null;
        }

        public void ClearChatFlow()
        {
            GameObjectUtils.DestroyAllChildrens(chatFlow.transform);
        }
        
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}