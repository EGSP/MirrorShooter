using UnityEngine;

namespace Game.EntitiesData.Weapon
{
    [CreateAssetMenu(fileName = "Magazine", menuName = "ScriptableObjects/Weapon/Magazine", order = 1)]
    public class MagazineScriptableObject : ScriptableObject
    {
        [SerializeField] private string weaponType;
        [SerializeField] private int ammoCount;
        
        
        public Magazine GetMagazineCopy()
        {
            return new Magazine(weaponType,ammoCount);
        }
    }

    public class Magazine
    {
        public Magazine(string weaponType, int ammoCount)
        {
            WeaponType = weaponType;
            AmmoCount = ammoCount;
        }
        
        /// <summary>
        /// Тип оружия, которому можно использовать магазин.
        /// </summary>
        public string WeaponType { get; private set; }
        
        /// <summary>
        /// Текущее количество боеприпасов.
        /// </summary>
        public int AmmoCount { get; set; }
    }
    
    
}