using System;
using Game.Entities;
using Game.Views.Client.Session;
using Sirenix.OdinInspector;

namespace Game.Ui.Controllers
{
    public class UWeaponShopController : SerializedMonoBehaviour
    {
        public WeaponShopMenuView View { get; set; }

        public WeaponShop Shop { get; set; }

        private void Awake()
        {
            View = GetComponent<WeaponShopMenuView>();
            if(View == null)
                throw new NullReferenceException();
        }
        
        
    }
}