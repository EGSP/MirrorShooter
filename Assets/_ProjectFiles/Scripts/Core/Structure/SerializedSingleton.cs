using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gasanov.Core
{
    public class SerializedSingleton<TSingleton> : SerializedMonoBehaviour where TSingleton : SerializedMonoBehaviour
    {
        public static TSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    if(!AllowLazyInstance)
                        throw new LazyInstanceException(typeof(TSingleton));
                    
                    _instance = FindObjectOfType<TSingleton>();

                    if (_instance == null)
                    {

                        var singGameObject = new GameObject("[Singleton]" + typeof(TSingleton));
                        _instance = singGameObject.AddComponent<TSingleton>();
                        _instanceGameObject = singGameObject;
                    }
                    else
                    {
                        _instanceGameObject = _instance.gameObject;
                    }
                }

                return _instance;
            }
            protected set => _instance = value;
        }

        private static TSingleton _instance;
        private static GameObject _instanceGameObject;

        /// <summary>
        /// Создание при первом обращении.
        /// </summary>
        protected static bool AllowLazyInstance
        {
            get
            {
                var lazyAttribute = 
                    (LazyInstanceAttribute)Attribute.GetCustomAttribute(typeof(TSingleton), typeof(LazyInstanceAttribute));

                if (lazyAttribute == null)
                    return true;

                return lazyAttribute.AllowLazyInstance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            var instance = Instance;
        }

        /// <summary>
        /// Помечает экземпляр как DontDestroyOnLoad.
        /// </summary>
        public static bool AlwaysExist
        {
            get => alwaysExist;
            set
            {
                if(value == true)
                    DontDestroyOnLoad(_instance);
            }
        }
        private static bool alwaysExist;

        
        /// <param name="immidiate">Уничтожить мгновенно, а не в конце кадра.</param>
        public static void DestroyIfExist(bool immidiate = false)
        {
            if (_instance != null)
            {
                if (immidiate)
                {
                    DestroyImmediate(_instanceGameObject);
                }
                else
                {
                    Destroy(_instanceGameObject);
                }
            }
        }

        public static void CreateInstance()
        {
            if (_instance != null)
            {
                if (_instanceGameObject != null)
                    DestroyImmediate(_instanceGameObject);

                _instance = null;
            }
            
            var singGameObject = new GameObject("[Singleton]" + typeof(TSingleton));
            _instance = singGameObject.AddComponent<TSingleton>();
            _instanceGameObject = singGameObject;
        }
        
    }
}