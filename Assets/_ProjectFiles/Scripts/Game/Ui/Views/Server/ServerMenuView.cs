using System;
using Egsp.Core.Ui;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Server
{
    public class ServerMenuView : SerializedView
    {
        public event Action OnStartSetupButton = delegate {  };
        public event Action OnExitButton = delegate {  };

        public event Action OnStartButton = delegate {  };
        public event Action OnBackToMenuButton = delegate {  };

        [BoxGroup("Windows")]
        public GameObject menuWindow;
        [BoxGroup("Windows")]
        public GameObject setupWindow;
        
        [BoxGroup("Connection setup")]
        public TMP_InputField portInput;
        
        public void ShowStartSetupWindow()
        {
            setupWindow.SetActive(true);
        }

        public void CloseStartSetupWindow()
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
        
        public void StartSetup()
        {
            OnStartSetupButton();
        }

        public void Exit()
        {
            OnExitButton();
        }
        
        public void StartServer()
        {

            if (portInput.text.IsNullOrWhitespace())
                return;

            OnStartButton();
        }
        
        public void BackToMenu()
        {
            OnBackToMenuButton();
        }
    }
}