using System;
using System.Collections;
using System.Collections.Generic;
using Gasanov.Core.Mvp;
using Gasanov.Utils.GameObjectUtilities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views
{
    public class MainMenuView : MonoBehaviour, IView
    {
        [BoxGroup("Login")]
        [SerializeField] private GameObject loginWindow;
        [BoxGroup("Login")]
        [SerializeField] private TMP_InputField loginField;

        [BoxGroup("Mode select")]
        [SerializeField] private GameObject clientModeSelect;
        [BoxGroup("Mode select")]
        [SerializeField] private GameObject serverModeSelect;

        [BoxGroup("Connect setup")] 
        [SerializeField] private GameObject connectWindow;
        [BoxGroup("Connect setup")]
        [SerializeField] private TMP_InputField connectionPortField;
        [BoxGroup("Connect setup")]
        [SerializeField] private TMP_InputField connectionAddressField;

        [BoxGroup("Server setup")] 
        [SerializeField] private GameObject serverWindow;
        [BoxGroup("Server setup")]
        [SerializeField] private TMP_InputField serverPortField;

        [FoldoutGroup("InfoPanel")]
        [SerializeField] GameObject infoPanel;
        [FoldoutGroup("InfoPanel")]
        [SerializeField] private TMP_Text textboxPrefab;

        [SerializeField] private TMP_Text statusTextbox;
        
        private bool isClientMode { get; set; }
        
        public event Action<string> OnLogin = delegate(string s) {  };
        public event Action OnRelogin = delegate {  };
        public event Action OnClientMode = delegate {  };
        public event Action OnConnectionQuit = delegate {  };
        public event Action OnServerMode = delegate {  };
        public event Action<string, string> OnConnect = delegate(string s, string s1) {  };
        public event Action<string> OnStartServer = delegate(string s) {  };
        public event Action OnExit = delegate {  };

        private Action clearLast;

        private Action backToPrevious;

        private void Awake()
        {
            loginField.text = "Jobert";
            
            connectionAddressField.text = "127.0.0.1";
            connectionPortField.text = "7777";

            serverPortField.text = "7777";
        }

        public void ClientLogin()
        {
            var name = loginField.text;
            if (name.IsNullOrWhitespace())
                return;

            isClientMode = true;
            OnLogin(name);
        }

        public void ServerLogin()
        {
            var name = "Server";
            isClientMode = false;
            OnLogin(name);
        }

        public void Relogin()
        {
            OnRelogin();
        }

        public void ClientMode()
        {
            OnClientMode();
        }

        public void ServerMode()
        {
            OnServerMode();
        }

        public void Connect()
        {
            if (connectionPortField.text.IsNullOrWhitespace())
                return;

            if (connectionAddressField.text.IsNullOrWhitespace())
                return;

            OnConnect(connectionAddressField.text, connectionPortField.text);
        }

        public void StartServer()
        {
            if (serverPortField.text.IsNullOrWhitespace())
                return;

            OnStartServer(serverPortField.text);
        }

        public void Exit()
        {
            OnExit();
        }
        
        
        public void ShowLoginWindow()
        {
            ClearLast();
            loginWindow.SetActive(true);

            clearLast = () => loginWindow.SetActive(false);
        }
        
        public void ShowModeSelectWindow()
        {
            ClearLast();
            if (isClientMode)
            {
                clientModeSelect.SetActive(true);

                clearLast = () => clientModeSelect.SetActive(false);
            }
            else
            {
                serverModeSelect.SetActive(true);

                clearLast = () => serverModeSelect.SetActive(false);
            }
        }

        public void ShowConnectWindow()
        {
            ClearLast();
            connectWindow.SetActive(true);

            clearLast = () => connectWindow.SetActive(false);
            backToPrevious = ()=>
            {
                OnConnectionQuit();
                ShowModeSelectWindow();
            };
        }

        public void ShowServerWindow()
        {
            ClearLast();
            serverWindow.SetActive(true);

            clearLast = () => serverWindow.SetActive(false);
            backToPrevious = ShowModeSelectWindow;
        }

        public void ShowInfo(params string[] info)
        {
            infoPanel.SetActive(true);
            ClearInfo();

            for (var i = 0; i < info.Length; i++)
            {
                var inst = Instantiate(textboxPrefab, infoPanel.transform, false);
                inst.text = info[i];
            }
        }

        public void ClearInfo()
        {
            GameObjectUtils.DestroyAllChildrens(infoPanel.transform);
        }

        public void SetStatus(string statusText)
        {
            statusTextbox.text = statusText;
        }


        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            clearLast();
            gameObject.SetActive(false);
        }
            
        private void ClearLast()
        {
            clearLast?.Invoke();
            clearLast = null;
        }

        public void BackToPrevious()
        {
            backToPrevious?.Invoke();
        }
    }
}
