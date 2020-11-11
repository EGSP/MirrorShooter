using Game.EntitiesData.Weapon;
using Game.Net.Objects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities
{
    public class Weapon : DualNetworkBehaviour
    {
        [BoxGroup("Spawn logic")]
        [Tooltip("Префаб магазина для данного оружия.")]
        [SerializeField] private MagazineScriptableObject magazineBlueprint;
        
        [BoxGroup("Spawn logic")]
        [Tooltip("Сколько магазинов выдается игроку при спавне оружия.")]
        [SerializeField] private int magazineStartCount;

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