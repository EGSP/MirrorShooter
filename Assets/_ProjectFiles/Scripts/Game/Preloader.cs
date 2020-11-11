using System.Collections;
using System.Collections.Generic;
using Game;
using Gasanov.Core;
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
        [OdinSerialize]
        public string SceneName { get; private set; }
        
        // Входная точка всей игры.
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            PreloaderBack.Awake(this);
            
            PreloaderBack.AfterAwake(this);
            
            LoadFirstScene(SceneName);
        }

        private void LoadFirstScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            
        }
        
        
    }
}