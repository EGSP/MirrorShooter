using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class SpawnPoint : MonoBehaviour
    {
        /// <summary>
        /// Все текущие точки спавна.
        /// </summary>
        public static List<SpawnPoint> SpawnPoints { get; private set; } = new List<SpawnPoint>();

        public void OnEnable()
        {
            SpawnPoints.Add(this);
        }
        
        public void OnDisable()
        {
            SpawnPoints.Remove(this);
        }
    }
}