using System;
using Game.Entities.Controllers;
using UnityEngine;

namespace Game.Presenters.Session
{
    [RequireComponent(typeof(ClientQuickMenuPresenter))]
    public class QuickMenuController : MonoBehaviour
    {
        [SerializeField] private KeyCode toogleKey = KeyCode.Escape;

        private ClientQuickMenuPresenter presenter;

        private void Awake()
        {
            presenter = GetComponent<ClientQuickMenuPresenter>();
            if (presenter == null)
            {
                Debug.LogWarning("Не найден presenter для quick_menu");
            }
        }

        private void Update()
        {
            if (presenter == null)
                return;
            
            if (Input.GetKeyDown(toogleKey))
            {
                if (presenter.IsActive)
                {
                    GlobalInput.SetCharacterMode();
                    presenter.Deactivate();
                }
                else
                {
                    GlobalInput.SetInterfaceMode();
                    presenter.Activate();
                }
            }
        }
    }
}