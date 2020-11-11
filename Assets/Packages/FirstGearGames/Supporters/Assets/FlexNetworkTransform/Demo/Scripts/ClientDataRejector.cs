using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.FlexNetworkTransforms.Demos
{

    public class ClientDataRejector : NetworkBehaviour
    {
        /// <summary>
        /// True to reject all data from a client.
        /// </summary>
        [Tooltip("True to reject all data from a client.")]
        [SerializeField]
        private bool _rejectData = false;
        /// <summary>
        /// True to reset client to the last accepted position when decling data.
        /// </summary>
        [Tooltip("True to reset client to the last accepted position when decling data.")]
        [SerializeField]
        private bool _resetClient = true;

        private FlexNetworkTransform _fnt;

        private void Awake()
        {
            _fnt = GetComponent<FlexNetworkTransform>();    
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            _fnt.OnClientDataReceived += FlexNetworkTransform_OnClientDatAReceived;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            _fnt.OnClientDataReceived -= FlexNetworkTransform_OnClientDatAReceived;
        }

        private void FlexNetworkTransform_OnClientDatAReceived(ReceivedClientData obj)
        {
            //Don't reject data.
            if (!_rejectData)
                return;

            if (_rejectData)
            {
                /* To reject data on the server you only have to nullify
                 * the data. For example: obj.Data = null; */

                /* You may also modify the data instead.
                 * For example: obj.Data.Position = Vector3.zero; */

                /* Be aware that data may arrive as LocalSpace or WorldSpace
                 * depending on your FNT settings. When modifying data be sure to
                 * convert when necessary. */

                /* You could even implement your own way of snapping the client
                 * authoritative player back after rejecting the data. In my example
                 * I send the current coordinates of the transform back to the client
                 * in which they teleport to these values. */

                if (_resetClient)
                    TargetReset(_fnt.TargetTransform.position, _fnt.TargetTransform.rotation, _fnt.TargetTransform.localScale);

                obj.Data = null;
            }
        }

        /// <summary>
        /// Resets client's TargetTransform using data.
        /// </summary>
        /// <param name="rcd"></param>
        [TargetRpc]
        private void TargetReset(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            _fnt.TargetTransform.position = position;
            _fnt.TargetTransform.rotation = rotation;
            _fnt.TargetTransform.localScale = scale;
        }
    }


}