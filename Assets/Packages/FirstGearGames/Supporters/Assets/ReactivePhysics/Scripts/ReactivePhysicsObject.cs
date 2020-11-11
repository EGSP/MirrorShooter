using FirstGearGames.Utilities.Maths;
using FirstGearGames.Utilities.Objects;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirstGearGames.Mirrors.Assets.ReactivePhyics
{

    public class ReactivePhysicsObject : NetworkBehaviour
    {
        #region Types.
        /// <summary>
        /// Type of object this is being used on.
        /// </summary>
        private enum ObjectTypes
        {
            Object3D = 0,
            Object2D = 1
        }
        /// <summary>
        /// Data sent to observers to keep object in sync.
        /// </summary>
        private struct SyncData
        {
            public SyncData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }

            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector2 Velocity2D { get { return new Vector2(Velocity.x, Velocity.y); } }

            public Vector3 AngularVelocity;
            public float AngularVelocity2D { get { return AngularVelocity.z; } }
        }
        /// <summary>
        /// Interval types to determine when to synchronize data.
        /// </summary>
        [System.Serializable]
        private enum IntervalTypes : int
        {
            Timed = 0,
            FixedUpdate = 1
        }
        #endregion

        #region Serialized.
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("True to synchronize using localSpace rather than worldSpace. If you are to child this object throughout it's lifespan using worldspace is recommended. However, when using worldspace synchronization may not behave properly on VR. LocalSpace is the default.")]
        [SerializeField]
        private bool _useLocalSpace = true;
        /// <summary>
        /// How to operate synchronization timings. Timed will synchronized every specified interval while FixedUpdate will synchronize every FixedUpdate.
        /// </summary>
        [Tooltip("How to operate synchronization timings. Timed will synchronized every specified interval while FixedUpdate will synchronize every FixedUpdate.")]
        [SerializeField]
        private IntervalTypes _intervalType = IntervalTypes.Timed;
        /// <summary>
        /// How often to synchronize this transform.
        /// </summary>
        [Tooltip("How often to synchronize this transform. If being used as a controller it's best to set this to the same rate that your controller sends movemenet.")]
        [Range(0.01f, 0.5f)]
        [FormerlySerializedAs("_syncInterval")]
        [SerializeField]
        private float _synchronizeInterval = 0.1f;
        /// <summary>
        /// True to synchronize using the reliable channel. False to synchronize using the unreliable channel. Your project must use 0 as reliable, and 1 as unreliable for this to function properly. This feature is not supported on TCP transports.
        /// </summary>
        [Tooltip("True to synchronize using the reliable channel. False to synchronize using the unreliable channel. Your project must use 0 as reliable, and 1 as unreliable for this to function properly.")]
        [SerializeField]
        private bool _reliable = true;
        /// <summary>
        /// True to synchronize data anytime it has changed. False to allow greater differences before synchronizing. Given that rigidbodies often shift continuously it's recommended to leave this false to not flood the network.
        /// </summary>
        [Tooltip("True to synchronize data anytime it has changed. False to allow greater differences before synchronizing. Given that rigidbodies often shift continuously it's recommended to leave this false to not flood the network.")]
        [SerializeField]
        private bool _preciseSynchronization = false;
        /// <summary>
        /// Indicates if this is a 2D or 3D object.
        /// </summary>
        [Tooltip("Indicates if this is a 2D or 3D object.")]
        [SerializeField]
        private ObjectTypes _objectType = ObjectTypes.Object3D;
        /// <summary>
        /// How strictly to synchronize this object when owner. Lower values will still keep the object in synchronization but it may take marginally longer for the object to correct if out of synchronization. It's recommended to use higher values, such as 0.5f, when using fast intervals. Default value is 0.1f.
        /// </summary>
        [Tooltip("How strictly to synchronize this object when owner. Lower values will still keep the object in synchronization but it may take marginally longer for the object to correct if out of synchronization. It's recommended to use higher values, such as 0.5f, when using fast intervals. Default value is 0.1f.")]
        [Range(0.01f, 0.75f)]
        [SerializeField]
        private float _strength = 0.5f;
        #endregion

        #region Private.
        /// <summary>
        /// SyncData client should move towards.
        /// </summary>
        private SyncData? _syncData = null;
        /// <summary>
        /// Rigidbody on this object. May be null.
        /// </summary>
        private Rigidbody _rigidbody;
        /// <summary>
        /// Rigidbody2D on this object. May be null.
        /// </summary>
        private Rigidbody2D _rigidbody2D;
        /// <summary>
        /// Last SyncData values sent by server.
        /// </summary>
        private SyncData _lastSentSyncData;
        /// <summary>
        /// Next time server can send SyncData.
        /// </summary>
        private double _nextSendTime = 0f;
        /// <summary>
        /// True if a reliable packet has been sent for most recent values.
        /// </summary>
        private bool _reliableSent = false;
        /// <summary>
        /// Last time SyncData was updated.
        /// </summary>
        private float _lastReceivedTime = 0f;
        #endregion

        #region Const.
        /// <summary>
        /// How much time must pass after receiving a packet before snapping to the last received value.
        /// </summary>
        private float TIME_PASSED_SNAP_VALUE = 3f;
        #endregion

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            /* Assign current data as a new sync data. Server will update as needed.
             * This is to stop player from moving objects which haven't received
             * an update yet. */
            _syncData = new SyncData(transform.GetPosition(_useLocalSpace), transform.GetRotation(_useLocalSpace), Vector3.zero, Vector3.zero);
        }

        private void FixedUpdate()
        {
            if (base.isServer)
            {
                CheckSendSyncData();
            }
            if (base.isClientOnly)
            {
                MoveTowardsSyncDatas();
            }
        }

        private void Update()
        {
            if (base.isServer)
            {
                CheckSendSyncData();
            }
            if (base.isClientOnly)
            {
                MoveTowardsSyncDatas();
            }
        }

        /// <summary>
        /// Temporarily reduces aggressive to stay in synchronization. Best to use this after your object is moved using it's controller.
        /// </summary>
        [Client]
        public void ReduceAggressiveness()
        {
            _lastReceivedTime = Time.time;
        }
        /// <summary>
        /// Clears data to synchronize towards allowing this gameObject to teleport. The object should be teleported on the server as well.
        /// </summary>
        [Client]
        public void AllowTeleport()
        {
            _syncData = null;
        }

        /// <summary>
        /// Moves towards most recent sync data values.
        /// </summary>
        private void MoveTowardsSyncDatas()
        {
            if (_syncData == null)
                return;
            if (_strength == 0f)
                return;
            //If data matches no reason to continue.
            if (SyncDataMatchesObject(_syncData.Value, _preciseSynchronization))
            {
                /* Also reset data received time so smoothing
                 * will reoccur if object is bumped into, rather
                 * than snapping to last position. */
                return;
            }

            float timePassed = Time.time - _lastReceivedTime;

            //If to snap.
            if (timePassed > TIME_PASSED_SNAP_VALUE)
            {
                transform.SetPosition(_useLocalSpace, _syncData.Value.Position);
                transform.SetRotation(_useLocalSpace, _syncData.Value.Rotation);
                //2D
                if (_objectType == ObjectTypes.Object2D)
                {
                    _rigidbody2D.velocity = _syncData.Value.Velocity2D;
                    _rigidbody2D.angularVelocity = _syncData.Value.AngularVelocity2D;
                    Physics2D.SyncTransforms();
                }
                //3D
                else
                {
                    _rigidbody.velocity = _syncData.Value.Velocity;
                    _rigidbody.angularVelocity = _syncData.Value.AngularVelocity;
                    Physics.SyncTransforms();
                }
            }
            //Do not snap yet.
            else
            {
                //If owner use configured strength, otherwise always use 1f.
                float strength = (base.hasAuthority) ? _strength : 1f;
                float accumulated = timePassed * strength;
                //Smoothing multiplier based on sync interval and frame rate.
                float deltaMultiplier = (Time.deltaTime / ReturnSyncInterval());
                float distance;

                //Modify transform properties in regular update for smoother visual transitions.
                if (!Time.inFixedTimeStep)
                {
                    //Position.
                    distance = Vector3.Distance(transform.GetPosition(_useLocalSpace), _syncData.Value.Position);
                    distance *= distance;
                    transform.SetPosition(_useLocalSpace,
                        Vector3.MoveTowards(transform.GetPosition(_useLocalSpace), _syncData.Value.Position, accumulated * distance));
                    //Rotation
                    distance = Quaternion.Angle(transform.rotation, _syncData.Value.Rotation);
                    transform.SetRotation(_useLocalSpace,
                        Quaternion.RotateTowards(transform.GetRotation(_useLocalSpace), _syncData.Value.Rotation, deltaMultiplier * distance * strength));
                    /* Only sync transforms if have authority. This is because
                     * if the client has authority we assume they have the ability
                     * to manipulate the objects transform. */
                    if (base.hasAuthority)
                    {
                        if (_objectType == ObjectTypes.Object3D)
                            Physics.SyncTransforms();
                        else if (_objectType == ObjectTypes.Object3D)
                            Physics2D.SyncTransforms();
                    }
                }
                //Move forces in fixed update.
                else
                {
                    //Velocity.
                    if (_objectType == ObjectTypes.Object2D)
                    {
                        //Angular.
                        distance = Mathf.Abs(_rigidbody2D.angularVelocity - _syncData.Value.AngularVelocity2D);
                        _rigidbody2D.angularVelocity = Mathf.MoveTowards(_rigidbody2D.angularVelocity, _syncData.Value.AngularVelocity2D, deltaMultiplier * distance * strength);
                        //Velocity.
                        distance = Vector2.Distance(_rigidbody2D.velocity, _syncData.Value.Velocity2D);
                        _rigidbody2D.velocity = Vector2.MoveTowards(_rigidbody2D.velocity, _syncData.Value.Velocity2D, deltaMultiplier * distance * strength);
                    }
                    else
                    {
                        //Angular.
                        distance = Vector3.Distance(_rigidbody.angularVelocity, _syncData.Value.AngularVelocity);
                        _rigidbody.angularVelocity = Vector3.MoveTowards(_rigidbody.angularVelocity, _syncData.Value.AngularVelocity, deltaMultiplier * distance * strength);
                        //Velocity
                        distance = Vector3.Distance(_rigidbody.velocity, _syncData.Value.Velocity);
                        _rigidbody.velocity = Vector3.MoveTowards(_rigidbody.velocity, _syncData.Value.Velocity, deltaMultiplier * distance * strength);
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns used synchronization interval.
        /// </summary>
        /// <returns></returns>
        private float ReturnSyncInterval()
        {
            if (_intervalType == IntervalTypes.FixedUpdate)
                return Time.fixedDeltaTime;
            else
                return _synchronizeInterval;
        }

        /// <summary>
        /// Returns if properties can be sent to clients.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="useReliable"></param>
        /// <returns></returns>
        private bool CanSendProperties(SyncData data, ref bool useReliable)
        {
            bool dataMatches = SyncDataMatchesObject(data, _preciseSynchronization);

            //Send if data doesn't match.
            if (!dataMatches)
            {
                /* Unset ReliableSent so that it will fire
                 * once object has settled, assuming not using a reliable
                 * transport. */
                _reliableSent = false;
                return true;
            }
            //If data matches.
            else
            {
                //If using unreliable, but reliable isn't sent yet.
                if (!_reliable && !_reliableSent)
                {
                    useReliable = true;
                    _reliableSent = true;
                    return true;
                }
                //Either using reliable or reliable already sent.
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns if the specified SyncData matches values on this object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SyncDataMatchesObject(SyncData data, bool precise)
        {
            bool transformMatches = (PositionMatches(data, precise) && RotationMatches(data, precise));

            bool velocityMatches = false;
            /* If transform matches then we must check
             * also if physics match. If transform does not match there's
             * no reason to check physics as an update is required regardless. */
            if (transformMatches)
                velocityMatches = VelocityMatches(data, precise);

            return (transformMatches && velocityMatches);
        }

        /// <summary>
        /// Returns if this transform position matches data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool PositionMatches(SyncData data, bool precise)
        {
            if (precise)
            {
                return (transform.GetPosition(_useLocalSpace) == data.Position);
            }
            else
            {
                float dist = Vector3.SqrMagnitude(transform.GetPosition(_useLocalSpace) - data.Position);
                return (dist < 0.0001f);
            }
        }

        /// <summary>
        /// Returns if this transform rotation matches data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool RotationMatches(SyncData data, bool precise)
        {
            if (precise)
                return transform.GetRotation(_useLocalSpace).Matches(data.Rotation);
            else
                return transform.GetRotation(_useLocalSpace).Matches(data.Rotation, 1f);
        }

        private bool VelocityMatches(SyncData data, bool precise)
        {
            //2D object.
            if (_objectType == ObjectTypes.Object2D)
            {
                if (precise)
                {
                    return ((_rigidbody2D.velocity == data.Velocity2D) && (_rigidbody2D.angularVelocity == data.AngularVelocity2D));
                }
                else
                {
                    float dist;
                    dist = Vector2.SqrMagnitude(_rigidbody2D.velocity - data.Velocity2D);
                    //If velocity is outside tolerance then return false early.
                    if (dist >= 0.0025f)
                        return false;
                    //No reason to square angular difference.
                    dist = Mathf.Abs((_rigidbody2D.angularVelocity - data.AngularVelocity2D));
                    return (dist < 0.05f);
                }
            }
            //3D object.
            else
            {
                if (precise)
                {
                    return ((_rigidbody.velocity == data.Velocity) && (_rigidbody.angularVelocity == data.AngularVelocity));
                }
                else
                {
                    float dist;
                    dist = Vector3.SqrMagnitude(_rigidbody.velocity - data.Velocity);
                    //If velocity is outside tolerance then return false early.
                    if (dist >= 0.0025f)
                        return false;
                    //Angular.
                    dist = Vector3.SqrMagnitude(_rigidbody.angularVelocity - data.AngularVelocity);
                    return (dist < 0.0025f);
                }
            }
        }

        /// <summary>
        /// Checks if SyncData needs to be sent over the network.
        /// </summary>
        private void CheckSendSyncData()
        {
            //Timed interval.
            if (_intervalType == IntervalTypes.Timed)
            {
                if (Time.inFixedTimeStep)
                    return;

                if (Time.time < _nextSendTime)
                    return;
            }
            //Fixed interval.
            else
            {
                if (!Time.inFixedTimeStep)
                    return;
            }

            bool useReliable = _reliable;
            //Values haven't changed.
            if (!CanSendProperties(_lastSentSyncData, ref useReliable))
                return;

            /* If here a new sync data needs to be sent. */

            //Set sync data being set, and next time data can send.
            Vector3 velocity;
            if (_objectType == ObjectTypes.Object2D)
                velocity = new Vector3(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y, 0f);
            else
                velocity = _rigidbody.velocity;

            Vector3 angularVelocity;
            if (_objectType == ObjectTypes.Object2D)
                angularVelocity = new Vector3(0f, 0f, _rigidbody2D.angularVelocity);
            else
                angularVelocity = _rigidbody.angularVelocity;

            _lastSentSyncData = new SyncData(transform.GetPosition(_useLocalSpace), transform.GetRotation(_useLocalSpace), velocity, angularVelocity);
            //Set time regardless if using interval or not. Quicker than running checks.
            _nextSendTime = NetworkTime.time + _synchronizeInterval;

            //Send new SyncData to clients.
            if (useReliable)
                RpcUpdateSyncDataReliable(_lastSentSyncData);
            else
                RpcUpdateSyncDataUnreliable(_lastSentSyncData);
        }

        /// <summary>
        /// Updates SyncData on clients.
        /// </summary>
        /// <param name="data"></param>
        [ClientRpc(channel = 0)]
        private void RpcUpdateSyncDataReliable(SyncData data)
        {
            ServerSyncDataReceived(data);
        }

        /// <summary>
        /// Updates SyncData on clients.
        /// </summary>
        /// <param name="data"></param>
        [ClientRpc(channel = 1)]
        private void RpcUpdateSyncDataUnreliable(SyncData data)
        {
            ServerSyncDataReceived(data);
        }

        /// <summary>
        /// Called when client 
        /// </summary>
        /// <param name="data"></param>
        private void ServerSyncDataReceived(SyncData data)
        {
            //If received on client host, no need to update.
            if (base.isServer)
                return;

            _lastReceivedTime = Time.time;
            _syncData = data;
        }

    }


}