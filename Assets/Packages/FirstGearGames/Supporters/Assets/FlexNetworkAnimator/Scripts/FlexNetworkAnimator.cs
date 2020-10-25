using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirstGearGames.Mirrors.Assets.FlexNetworkAnimators
{
    /* This is a derivative, and not a full rewrite of NetworkAnimator. */


    /// <summary>
    /// A component to synchronize Mecanim animation states for networked objects.
    /// </summary>
    /// <remarks>
    /// <para>The animation of game objects can be networked by this component. There are two models of authority for networked movement:</para>
    /// <para>If the object has authority on the client, then it should be animated locally on the owning client. The animation state information will be sent from the owning client to the server, then broadcast to all of the other clients. This is common for player objects.</para>
    /// <para>If the object has authority on the server, then it should be animated on the server and state information will be sent to all clients. This is common for objects not related to a specific client, such as an enemy unit.</para>
    /// <para>The NetworkAnimator synchronizes all animation parameters of the selected Animator. It does not automatically sychronize triggers. The function SetTrigger can by used by an object with authority to fire an animation trigger on other clients.</para>
    /// </remarks>
    [AddComponentMenu("Network/FlexNetworkAnimator")]
    [RequireComponent(typeof(NetworkIdentity))]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkAnimator.html")]
    public class FlexNetworkAnimator : NetworkBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(FlexNetworkAnimator));

        #region Types.
        /// <summary>
        /// Data about how to lerp a float value.
        /// </summary>
        private struct LerpingFloat
        {
            public LerpingFloat(float rate, float target)
            {
                Rate = rate;
                Target = target;
            }

            public readonly float Rate;
            public readonly float Target;
        }
        #endregion

        #region Serialized.
        /// <summary>
        /// The animator component to synchronize.
        /// </summary>
        [FormerlySerializedAs("m_Animator")]
        [Tooltip("Animator that will have parameters synchronized")]
        [SerializeField]
        private Animator _animator;
        /// <summary>
        /// The animator component to synchronize.
        /// </summary>
        public Animator Animator { get { return _animator; } }
        /// <summary>
        /// True if using client authoritative animations.
        /// </summary>
        [Tooltip("True if using client authoritative animations.")]
        [SerializeField]
        private bool _clientAuthoritative = true;
        /// <summary>
        /// True to synchronize server results back to owner. Typically used when you are changing animations on the server and are relying on the server response to update the clients animations.
        /// </summary>
        [Tooltip("True to synchronize server results back to owner. Typically used when you are changing animations on the server and are relying on the server response to update the clients animations.")]
        [SerializeField]
        private bool _synchronizeToOwner = false;
        /// <summary>
        /// True to smooth floats on spectators rather than snap to their values immediately. Commonly set to true for smooth blend tree animations.
        /// </summary>
        [Tooltip("True to smooth floats on spectators rather than snap to their values immediately. Commonly set to true for smooth blend tree animations.")]
        [SerializeField]
        private bool _smoothFloats = true;
        /// <summary>
        /// How much time to fall behind when using smoothing. Only increase value if the smoothing is sometimes jittery. Recommended values are between 0 and 0.04.
        /// </summary>
        [Tooltip("How much time to fall behind when using smoothing. Only increase value if the smoothing is sometimes jittery. Recommended values are between 0 and 0.04.")]
        [Range(0f, 0.1f)]
        [SerializeField]
        private float _interpolationFallbehind = 0.02f;
        #endregion

        #region Private.
        /// <summary>
        /// Current values of floats to smooth towards.
        /// </summary>
        private Dictionary<int, LerpingFloat> _floatTargets;
        // Note: not an object[] array because otherwise initialization is real annoying
        private int[] lastIntParameters;
        private float[] lastFloatParameters;
        private bool[] lastBoolParameters;
        private AnimatorControllerParameter[] parameters;
        #endregion

        

        // multiple layers
        private int[] animationHash;
        private int[] transitionHash;
        private float[] layerWeight;
        private float sendTimer;

        bool SendMessagesAllowed
        {
            get
            {
                if (isServer)
                {
                    if (!_clientAuthoritative)
                        return true;

                    // This is a special case where we have client authority but we have not assigned the client who has
                    // authority over it, no animator data will be sent over the network by the server.
                    //
                    // So we check here for a connectionToClient and if it is null we will
                    // let the server send animation data until we receive an owner.
                    if (netIdentity != null && netIdentity.connectionToClient == null)
                        return true;
                }

                return (hasAuthority && _clientAuthoritative);
            }
        }

        void Awake()
        {
            // store the animator parameters in a variable - the "Animator.parameters" getter allocates
            // a new parameter array every time it is accessed so we should avoid doing it in a loop
            parameters = _animator.parameters
                .Where(par => !_animator.IsParameterControlledByCurve(par.nameHash))
                .ToArray();
            _floatTargets = new Dictionary<int, LerpingFloat>();
            lastIntParameters = new int[parameters.Length];
            lastFloatParameters = new float[parameters.Length];
            lastBoolParameters = new bool[parameters.Length];

            animationHash = new int[_animator.layerCount];
            transitionHash = new int[_animator.layerCount];
            layerWeight = new float[_animator.layerCount];
        }

        void FixedUpdate()
        {
            if (!SendMessagesAllowed)
                return;

            CheckSendRate();

            for (int i = 0; i < _animator.layerCount; i++)
            {
                int stateHash;
                float normalizedTime;
                if (!CheckAnimStateChanged(out stateHash, out normalizedTime, i))
                {
                    continue;
                }

                using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
                {
                    WriteParameters(writer);
                    SendAnimationMessage(stateHash, normalizedTime, i, layerWeight[i], writer.ToArray());
                }
            }
        }

        private void Update()
        {
            if (base.hasAuthority)
                return;

            if (_floatTargets.Count > 0)
            {
                float deltaTime = Time.deltaTime;

                List<int> finishedEntries = new List<int>();

                /* Cycle through each target float and move towards it.
                    * Once at a target float mark it to be removed from floatTargets. */
                foreach (KeyValuePair<int, LerpingFloat> item in _floatTargets)
                {
                    float current = _animator.GetFloat(item.Key);
                    float next = Mathf.MoveTowards(current, item.Value.Target, item.Value.Rate * deltaTime);
                    _animator.SetFloat(item.Key, next);

                    if (next == item.Value.Target)
                        finishedEntries.Add(item.Key);
                }

                //Remove finished entries from dictionary.
                for (int i = 0; i < finishedEntries.Count; i++)
                    _floatTargets.Remove(finishedEntries[i]);
            }
        }

        bool CheckAnimStateChanged(out int stateHash, out float normalizedTime, int layerId)
        {
            bool change = false;
            stateHash = 0;
            normalizedTime = 0;

            float lw = _animator.GetLayerWeight(layerId);
            if (lw != layerWeight[layerId])
            {
                layerWeight[layerId] = lw;
                change = true;
            }

            if (_animator.IsInTransition(layerId))
            {
                AnimatorTransitionInfo tt = _animator.GetAnimatorTransitionInfo(layerId);
                if (tt.fullPathHash != transitionHash[layerId])
                {
                    // first time in this transition
                    transitionHash[layerId] = tt.fullPathHash;
                    animationHash[layerId] = 0;
                    return true;
                }
                return change;
            }

            AnimatorStateInfo st = _animator.GetCurrentAnimatorStateInfo(layerId);
            if (st.fullPathHash != animationHash[layerId])
            {
                // first time in this animation state
                if (animationHash[layerId] != 0)
                {
                    // came from another animation directly - from Play()
                    stateHash = st.fullPathHash;
                    normalizedTime = st.normalizedTime;
                }
                transitionHash[layerId] = 0;
                animationHash[layerId] = st.fullPathHash;
                return true;
            }
            return change;
        }

        void CheckSendRate()
        {
            if (SendMessagesAllowed && syncInterval > 0 && sendTimer < Time.time)
            {
                sendTimer = Time.time + syncInterval;

                using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
                {
                    if (WriteParameters(writer))
                        SendAnimationParametersMessage(writer.ToArray());
                }
            }
        }

        void SendAnimationMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
        {
            if (isServer)
            {
                RpcOnAnimationClientMessage(stateHash, normalizedTime, layerId, weight, parameters);
            }
            else if (ClientScene.readyConnection != null)
            {
                CmdOnAnimationServerMessage(stateHash, normalizedTime, layerId, weight, parameters);
            }
        }

        void SendAnimationParametersMessage(byte[] parameters)
        {
            if (isServer)
            {
                RpcOnAnimationParametersClientMessage(parameters);
            }
            else if (ClientScene.readyConnection != null)
            {
                CmdOnAnimationParametersServerMessage(parameters);
            }
        }

        void HandleAnimMsg(int stateHash, float normalizedTime, int layerId, float weight, NetworkReader reader)
        {
            if (hasAuthority && _clientAuthoritative)
                return;

            // usually transitions will be triggered by parameters, if not, play anims directly.
            // NOTE: this plays "animations", not transitions, so any transitions will be skipped.
            // NOTE: there is no API to play a transition(?)
            if (stateHash != 0 && _animator.enabled)
            {
                _animator.Play(stateHash, layerId, normalizedTime);
            }

            _animator.SetLayerWeight(layerId, weight);

            ReadParameters(reader);
        }

        void HandleAnimParamsMsg(NetworkReader reader)
        {
            if (hasAuthority && _clientAuthoritative)
                return;

            ReadParameters(reader);
        }

        void HandleAnimTriggerMsg(int hash)
        {
            _animator.SetTrigger(hash);
        }

        void HandleAnimResetTriggerMsg(int hash)
        {
            _animator.ResetTrigger(hash);
        }

        ulong NextDirtyBits()
        {
            ulong dirtyBits = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter par = parameters[i];
                bool changed = false;
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = _animator.GetInteger(par.nameHash);
                    changed = newIntValue != lastIntParameters[i];
                    lastIntParameters[i] = newIntValue;
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = _animator.GetFloat(par.nameHash);
                    changed = newFloatValue != lastFloatParameters[i];
                    lastFloatParameters[i] = newFloatValue;
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = _animator.GetBool(par.nameHash);
                    changed = newBoolValue != lastBoolParameters[i];
                    lastBoolParameters[i] = newBoolValue;
                }
                if (changed)
                {
                    dirtyBits |= 1ul << i;
                }
            }
            return dirtyBits;
        }

        bool WriteParameters(NetworkWriter writer, bool forceAll = false)
        {
            ulong dirtyBits = forceAll ? (~0ul) : NextDirtyBits();
            writer.WritePackedUInt64(dirtyBits);
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((dirtyBits & (1ul << i)) == 0)
                    continue;

                AnimatorControllerParameter par = parameters[i];
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = _animator.GetInteger(par.nameHash);
                    writer.WritePackedInt32(newIntValue);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = _animator.GetFloat(par.nameHash);
                    writer.WriteSingle(newFloatValue);
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = _animator.GetBool(par.nameHash);
                    writer.WriteBoolean(newBoolValue);
                }
            }
            return dirtyBits != 0;
        }

        void ReadParameters(NetworkReader reader)
        {
            ulong dirtyBits = reader.ReadPackedUInt64();
            for (int i = 0; i < parameters.Length; i++)
            {
                if ((dirtyBits & (1ul << i)) == 0)
                    continue;

                AnimatorControllerParameter par = parameters[i];
                if (par.type == AnimatorControllerParameterType.Int)
                {
                    int newIntValue = reader.ReadPackedInt32();
                    _animator.SetInteger(par.nameHash, newIntValue);
                }
                else if (par.type == AnimatorControllerParameterType.Float)
                {
                    float newFloatValue = reader.ReadSingle();

                    if (_smoothFloats)
                    {
                        float currentValue = _animator.GetFloat(par.nameHash);
                        float past = base.syncInterval + _interpolationFallbehind;
                        float rate = Mathf.Abs(currentValue - newFloatValue) / past;
                        _floatTargets[par.nameHash] = new LerpingFloat(rate, newFloatValue);
                    }
                    else
                    {
                        _animator.SetFloat(par.nameHash, newFloatValue);
                    }
                }
                else if (par.type == AnimatorControllerParameterType.Bool)
                {
                    bool newBoolValue = reader.ReadBoolean();
                    _animator.SetBool(par.nameHash, newBoolValue);
                }
            }
        }

        /// <summary>
        /// Custom Serialization
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="initialState"></param>
        /// <returns></returns>
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                for (int i = 0; i < _animator.layerCount; i++)
                {
                    if (_animator.IsInTransition(i))
                    {
                        AnimatorStateInfo st = _animator.GetNextAnimatorStateInfo(i);
                        writer.WriteInt32(st.fullPathHash);
                        writer.WriteSingle(st.normalizedTime);
                    }
                    else
                    {
                        AnimatorStateInfo st = _animator.GetCurrentAnimatorStateInfo(i);
                        writer.WriteInt32(st.fullPathHash);
                        writer.WriteSingle(st.normalizedTime);
                    }
                    writer.WriteSingle(_animator.GetLayerWeight(i));
                }
                WriteParameters(writer, initialState);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Custom Deserialization
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="initialState"></param>
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                for (int i = 0; i < _animator.layerCount; i++)
                {
                    int stateHash = reader.ReadInt32();
                    float normalizedTime = reader.ReadSingle();
                    _animator.SetLayerWeight(i, reader.ReadSingle());
                    _animator.Play(stateHash, i, normalizedTime);
                }

                ReadParameters(reader);
            }
        }

        /// <summary>
        /// Causes an animation trigger to be invoked for a networked object.
        /// <para>If local authority is set, and this is called from the client, then the trigger will be invoked on the server and all clients. If not, then this is called on the server, and the trigger will be called on all clients.</para>
        /// </summary>
        /// <param name="triggerName">Name of trigger.</param>
        public void SetTrigger(string triggerName)
        {
            SetTrigger(Animator.StringToHash(triggerName));
        }

        /// <summary>
        /// Causes an animation trigger to be invoked for a networked object.
        /// </summary>
        /// <param name="hash">Hash id of trigger (from the Animator).</param>
        public void SetTrigger(int hash)
        {
            if (_clientAuthoritative)
            {
                if (!isClient)
                {
                    logger.LogWarning("Tried to set animation in the server for a client-controlled animator");
                    return;
                }

                if (!hasAuthority)
                {
                    logger.LogWarning("Only the client with authority can set animations");
                    return;
                }

                if (ClientScene.readyConnection != null)
                    CmdOnAnimationTriggerServerMessage(hash);
            }
            else
            {
                if (!isServer)
                {
                    logger.LogWarning("Tried to set animation in the client for a server-controlled animator");
                    return;
                }

                RpcOnAnimationTriggerClientMessage(hash);
            }
        }

        /// <summary>
        /// Causes an animation trigger to be reset for a networked object.
        /// <para>If local authority is set, and this is called from the client, then the trigger will be reset on the server and all clients. If not, then this is called on the server, and the trigger will be reset on all clients.</para>
        /// </summary>
        /// <param name="triggerName">Name of trigger.</param>
        public void ResetTrigger(string triggerName)
        {
            ResetTrigger(Animator.StringToHash(triggerName));
        }

        /// <summary>
        /// Causes an animation trigger to be reset for a networked object.
        /// </summary>
        /// <param name="hash">Hash id of trigger (from the Animator).</param>
        public void ResetTrigger(int hash)
        {
            if (_clientAuthoritative)
            {
                if (!isClient)
                {
                    logger.LogWarning("Tried to reset animation in the server for a client-controlled animator");
                    return;
                }

                if (!hasAuthority)
                {
                    logger.LogWarning("Only the client with authority can reset animations");
                    return;
                }

                if (ClientScene.readyConnection != null)
                    CmdOnAnimationResetTriggerServerMessage(hash);
            }
            else
            {
                if (!isServer)
                {
                    logger.LogWarning("Tried to reset animation in the client for a server-controlled animator");
                    return;
                }

                RpcOnAnimationResetTriggerClientMessage(hash);
            }
        }

        #region server message handlers

        [Command]
        void CmdOnAnimationServerMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
        {
            // Ignore messages from client if not in client authority mode
            if (!_clientAuthoritative)
                return;

            if (logger.LogEnabled()) logger.Log("OnAnimationMessage for netId=" + netId);

            // handle and broadcast
            using (PooledNetworkReader networkReader = NetworkReaderPool.GetReader(parameters))
            {
                HandleAnimMsg(stateHash, normalizedTime, layerId, weight, networkReader);
                RpcOnAnimationClientMessage(stateHash, normalizedTime, layerId, weight, parameters);
            }
        }

        [Command]
        void CmdOnAnimationParametersServerMessage(byte[] parameters)
        {
            // Ignore messages from client if not in client authority mode
            if (!_clientAuthoritative)
                return;

            // handle and broadcast
            using (PooledNetworkReader networkReader = NetworkReaderPool.GetReader(parameters))
            {
                HandleAnimParamsMsg(networkReader);
                RpcOnAnimationParametersClientMessage(parameters);
            }
        }

        [Command]
        void CmdOnAnimationTriggerServerMessage(int hash)
        {
            // Ignore messages from client if not in client authority mode
            if (!_clientAuthoritative)
                return;

            // handle and broadcast
            HandleAnimTriggerMsg(hash);
            RpcOnAnimationTriggerClientMessage(hash);
        }

        [Command]
        void CmdOnAnimationResetTriggerServerMessage(int hash)
        {
            // Ignore messages from client if not in client authority mode
            if (!_clientAuthoritative)
                return;

            // handle and broadcast
            HandleAnimResetTriggerMsg(hash);
            RpcOnAnimationResetTriggerClientMessage(hash);
        }

        #endregion

        #region client message handlers

        [ClientRpc]
        void RpcOnAnimationClientMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
        {
            if (_clientAuthoritative && !_synchronizeToOwner && base.hasAuthority)
                return;

            using (PooledNetworkReader networkReader = NetworkReaderPool.GetReader(parameters))
                HandleAnimMsg(stateHash, normalizedTime, layerId, weight, networkReader);
        }

        [ClientRpc]
        void RpcOnAnimationParametersClientMessage(byte[] parameters)
        {
            if (_clientAuthoritative && !_synchronizeToOwner && base.hasAuthority)
                return;

            using (PooledNetworkReader networkReader = NetworkReaderPool.GetReader(parameters))
                HandleAnimParamsMsg(networkReader);
        }

        [ClientRpc]
        void RpcOnAnimationTriggerClientMessage(int hash)
        {
            if (_clientAuthoritative && !_synchronizeToOwner && base.hasAuthority)
                return;

            if (isServer) return;

            HandleAnimTriggerMsg(hash);
        }

        [ClientRpc]
        void RpcOnAnimationResetTriggerClientMessage(int hash)
        {
            if (_clientAuthoritative && !_synchronizeToOwner && base.hasAuthority)
                return;

            if (isServer) return;

            HandleAnimResetTriggerMsg(hash);
        }

        protected virtual void Reset()
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();
        }
        #endregion
    }
}

