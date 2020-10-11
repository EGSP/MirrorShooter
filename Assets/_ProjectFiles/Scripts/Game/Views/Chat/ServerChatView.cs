using Game.Net;
using Gasanov.Core.Mvp;
using Gasanov.Utils.GameObjectUtilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Views.Chat
{
    public class ServerChatView : MonoBehaviour, IView
    {
        [SerializeField] private GameObject chatFlow;
        
        [SerializeField] private int maxTextboxes;
        
        [SerializeField] private TMP_Text textboxPrefab;

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