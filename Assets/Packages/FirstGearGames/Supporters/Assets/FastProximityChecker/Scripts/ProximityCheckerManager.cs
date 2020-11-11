using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.NetworkProximities
{


    public class ProximityCheckerManager : MonoBehaviour
    {

        #region Private.
        /// <summary>
        /// Singleton reference to this script.
        /// </summary>
        private static ProximityCheckerManager _instance;
        /// <summary>
        /// Current active checkers.
        /// </summary>
        private List<FastProximityChecker> _checkers = new List<FastProximityChecker>();
        /// <summary>
        /// Index in Checkers to start on next cycle.
        /// </summary>
        private int _nextIndex;
        #endregion

        #region Const.
        /// <summary>
        /// Frames to spread updates over. Lower values will result in faster updates but will cost more performance.
        /// </summary>
        private const int TARGET_FPS = 50;
        #endregion

        private void Awake()
        {
            _instance = this;
        }


        private void Update()
        {
            CheckUpdateCheckers();
        }

        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FirstInitialize()
        {
            GameObject go = new GameObject();
            go.name = "ProximityCheckerManager";
            go.AddComponent<ProximityCheckerManager>();
            DontDestroyOnLoad(go);
        }

        /// <summary>
        /// Adds a BetterProximityChecker to collection.
        /// </summary>
        /// <param name="checker"></param>
        public static void AddChecker(FastProximityChecker checker)
        {
            _instance._checkers.Add(checker);
        }

        /// <summary>
        /// Removes a BetterProximityChecker from collection.
        /// </summary>
        /// <param name="checker"></param>
        public static void RemoveChecker(FastProximityChecker checker)
        {
            int index = _instance._checkers.IndexOf(checker);
            if (index != -1)
            {
                if (index < _instance._nextIndex)
                    _instance._nextIndex--;

                _instance._checkers.RemoveAt(index);
            }
        }

        /// <summary>
        /// Checks to update checkers.
        /// </summary>
        private void CheckUpdateCheckers()
        {
            if (!NetworkServer.active)
                return;

            UpdateCheckers();
        }

        /// <summary>
        /// Updates checkers.
        /// </summary>
        private void UpdateCheckers()
        {
            /* Performing one additional iteration would
            * likely be quicker than casting two ints
            * to a float. */
            int iterations = (_checkers.Count / TARGET_FPS) + 1;

            if (iterations > _checkers.Count)
                iterations = _checkers.Count;

            //Index to perform a check on.
            int checkerIndex = 0;
            /* Run the number of calculated iterations.
             * This is spaced out over frames to prevent
             * fps spikes. */
            for (int i = 0; i < iterations; i++)
            {
                checkerIndex = _nextIndex + i;
                if (checkerIndex >= _checkers.Count)
                    checkerIndex -= _checkers.Count;

                _checkers[checkerIndex].PerformCheck();
            }

            _nextIndex = (checkerIndex + 1);
        }
    }


}