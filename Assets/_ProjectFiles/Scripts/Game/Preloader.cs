using System.Collections;
using System.Collections.Generic;
using Game;
using Gasanov.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// Служит входной точкой игры. Также предоставляет доступ к функция MonoBehaviour.
    /// </summary>
    public class Preloader : SerializedSingleton<Preloader>
    {
        [OdinSerialize] public string FirstScene { get; private set; }
        
        [PropertySpace(10)]
        [InfoBox("Используется клиентской сессией при отключении от сервера.")]
        [OdinSerialize] public string OfflineScene { get; private set; }
        
        // Входная точка всей игры.
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            PreloaderBack.Awake(this);
            
            PreloaderBack.AfterAwake(this);
            
            LoadFirstScene(FirstScene);
        }

        private void LoadFirstScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        
    }
}