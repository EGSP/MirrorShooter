using System;
using Game.Entities;
using Game.Ui.Elements;
using Gasanov.Core.Mvp;
using Gasanov.Core.Pooling;

namespace Game.Views.Client.Session
{
    public class WeaponShopMenuView : SerializedView
    {
        public event Action<string> OnWeaponChoosed = delegate(string s) {  };
        
        public event Action OnClose = delegate {  };

        public WeaponElement elementPrefab;
        private SinglePoolContainer<WeaponElement> weaponPool;

        private void Awake()
        {
            weaponPool = SinglePoolContainer<WeaponElement>.CreateContainer("WeaponIcons");
            weaponPool.InitializePool(10, InitializeFunc);
        }

        private void OnWeaponClicked(WeaponElement clicked)
        {
            OnWeaponChoosed(clicked.TextWithId.text);
        }

        public void Close()
        {
            OnClose();
        }

        private WeaponElement InitializeFunc(int arg)
        {
            var weaponElement = Instantiate(elementPrefab);
            weaponElement.Button.onClick.AddListener(()=> OnWeaponClicked(weaponElement));
            return Instantiate(elementPrefab);
        }
    }
}