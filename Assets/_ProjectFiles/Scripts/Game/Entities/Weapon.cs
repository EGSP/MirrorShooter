using System;
using FirstGearGames.Mirrors.Assets.FlexNetworkTransforms;
using Game.EntitiesData.Weapon;
using Game.Net.Objects;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities
{
    [RequireComponent(typeof(FlexNetworkTransform))]
    public class Weapon : DualNetworkBehaviour
    {
        [OdinSerialize]
        public string Name { get; private set; }

        [BoxGroup("Spawn logic")]
        [Tooltip("Префаб магазина для данного оружия.")]
        [SerializeField] private MagazineScriptableObject magazineBlueprint;
        
        [BoxGroup("Spawn logic")]
        [Tooltip("Сколько магазинов выдается игроку при спавне оружия.")]
        [SerializeField] private int magazineStartCount;
        
        public event Action<Weapon> OnDroped = delegate(Weapon weapon) {  };
        public event Action<Weapon> OnTaked = delegate(Weapon weapon) {  };

        private FlexNetworkTransform networkTransform;


        public override void AwakeOnClient()
        {
            CommonAwake();
        }

        public override void AwakeOnServer()
        {
            CommonAwake();
        }

        private void CommonAwake()
        {
            networkTransform = GetComponent<FlexNetworkTransform>();
        }

        [Server]
        public void OnTake()
        {
            networkTransform.enabled = false;
            OnTaked(this);
        }

        [Server]
        public void OnDrop()
        {
            networkTransform.enabled = true;
            OnDroped(this);
        }
        
        /// <summary>
        /// Получение всех стартовых магазинов.
        /// </summary>
        public Magazine[] LoadMagazines()
        {
            if (magazineBlueprint == null)
                return null;

            if (magazineStartCount == 0)
                return null;

            var magazines = new Magazine[magazineStartCount];
            for (var i = 0; i < magazineStartCount; i++)
            {
                magazines[i] = magazineBlueprint.GetMagazineCopy();   
            }

            return magazines;
        }
    }
}