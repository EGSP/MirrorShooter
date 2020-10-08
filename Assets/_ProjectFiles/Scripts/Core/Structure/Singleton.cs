using UnityEngine;

namespace Gasanov.Core
{
    public abstract class Singleton<TSingleton> : MonoBehaviour where TSingleton : MonoBehaviour
    {
        public static TSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
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

        protected virtual void Awake()
        {
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
    }
}