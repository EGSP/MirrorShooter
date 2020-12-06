using System;
using Gasanov.Core.Mvp;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Client
{
    public class ClientMenuView : SerializedView
    {
        public event Action OnConnectSetupButton = delegate {  };
        public event Action OnExitButton = delegate {  };

        public event Action OnConnectButton = delegate {  };
        public event Action OnBackToMenuButton = delegate {  };

        [BoxGroup("Windows")]
        public GameObject menuWindow;
        [BoxGroup("Windows")]
        public GameObject setupWindow;
        
        [BoxGroup("Connection setup")]
        public TMP_InputField addressInput;
        [BoxGroup("Connection setup")]
        public TMP_InputField portInput;
        [BoxGroup("Connection setup")]
        public TMP_InputField nicknameInput;
        

        public void ShowConnectionSetupWindow()
        {
            setupWindow.SetActive(true);
        }

        public void CloseConnectionSetupWindow()
        {
            setupWindow.SetActive(false);
        }

        public void ShowMenuWindow()
        {
            menuWindow.SetActive(true);
        }

        public void CloseMenuWindow()
        {
            menuWindow.SetActive(false);
        }
        
        public void ConnectSetup()
        {
            OnConnectSetupButton();
        }

        public void Exit()
        {
            OnExitButton();
        }

        public void Connect()
        {
            if (addressInput.text.IsNullOrWhitespace())
                return;

            if (portInput.text.IsNullOrWhitespace())
                return;

            if (nicknameInput.text.IsNullOrWhitespace())
                return;
            
            OnConnectButton();
        }

        public void BackToMenu()
        {
            OnBackToMenuButton();
        }
    }
}