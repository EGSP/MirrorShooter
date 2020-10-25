using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.NetworkProximities
{
    /// <summary>
    /// Component that controls visibility of networked objects for players.
    /// <para>Any object with this component on it will not be visible to players more than a (configurable) distance away.</para>
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public class FastProximityChecker : NetworkVisibility
    {
        /// <summary>
        /// True to continuously update network visibility. False to only update on creation or when PerformCheck is called.
        /// </summary>
        [Tooltip("True to continuously update network visibility. False to only update on creation or when PerformCheck is called.")]
        [SerializeField]
        private bool _continuous = true;
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("The maximum range that objects will be visible at.")]
        [SerializeField]
        private int _visibilityRange = 10;
        /// <summary>
        /// The maximum range that objects will be visible at.
        /// </summary>
        public int VisibilityRange
        {
            get { return _visibilityRange; }
            set
            {
                _visibilityRange = value;
                SquareRange();
            }
        }
        /// <summary>
        /// Flag to force this object to be hidden for players.
        /// <para>If this object is a player object, it will not be hidden for that player.</para>
        /// </summary>
        [Tooltip("Enable to force this object to be hidden from players.")]
        public bool _forceHidden;

        /// <summary>
        /// Squared value of visibility range.
        /// </summary>
        private float _squaredVisibilityRange;

        private void Awake()
        {
            SquareRange();
        }

        private void OnValidate()
        {
            SquareRange();
        }

        /// <summary>
        /// Squares current visibility range for testing.
        /// </summary>
        private void SquareRange()
        {
            _squaredVisibilityRange = (_visibilityRange * _visibilityRange);
        }

        private void OnEnable()
        {
            if (_continuous)
                ProximityCheckerManager.AddChecker(this);
        }

        private void OnDisable()
        {
            if (_continuous)
                ProximityCheckerManager.RemoveChecker(this);
        }

        public void PerformCheck()
        {
            base.netIdentity.RebuildObservers(false);
        }

        /// <summary>
        /// Called when a new player enters
        /// </summary>
        /// <param name="newObserver">NetworkConnection of player object</param>
        /// <returns>True if object is within visible range</returns>
        public override bool OnCheckObserver(NetworkConnection newObserver)
        {
            if (_forceHidden)
                return false;

            return Vector3.SqrMagnitude(newObserver.identity.transform.position - transform.position) < _squaredVisibilityRange;
        }

        /// <summary>
        /// Called when a new player enters, and when scene changes occur
        /// </summary>
        /// <param name="observers">List of players to be updated.  Modify this set with all the players that can see this object</param>
        /// <param name="initial">True if this is the first time the method is called for this object</param>
        /// <returns>True if this component calculated the list of observers</returns>
        public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initial)
        {
            //If force hidden then return without adding any observers.
            if (_forceHidden)
                return;

            Vector3 position = transform.position;
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {

                if (conn == null || conn.identity == null)
                    continue;

                if (Vector3.SqrMagnitude(position - conn.identity.transform.position) < _squaredVisibilityRange)
                    observers.Add(conn);
            }
        }

        /// <summary>
        /// Called when hiding and showing objects on the host.
        /// On regular clients, objects simply spawn/despawn.
        /// On host, objects need to remain in scene because the host is also the server.
        ///    In that case, we simply hide/show meshes for the host player.
        /// </summary>
        /// <param name="visible"></param>
        public override void OnSetHostVisibility(bool visible)
        {
            foreach (Renderer rend in GetComponentsInChildren<Renderer>())
                rend.enabled = visible;
        }
    }
}
