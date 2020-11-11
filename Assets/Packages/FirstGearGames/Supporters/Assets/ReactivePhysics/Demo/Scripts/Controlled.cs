using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.ReactivePhyics.Demos
{

    public class Controlled : NetworkBehaviour
    {
        #region Serialized.
        /// <summary>
        /// How much force to apply towards move direction.
        /// </summary>
        [Tooltip("How much force to apply towards move direction.")]
        [SerializeField]
        private float _directionalForce = 1f;
        #endregion

        #region Private.
        /// <summary>
        /// Rigidbody on this object.
        /// </summary>
        private Rigidbody _rigidbody;
        /// <summary>
        /// ReactivePhysicsObject on this object
        /// </summary>
        private ReactivePhysicsObject _rpo;
        #endregion

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            _rigidbody = GetComponent<Rigidbody>();
            _rpo = GetComponent<ReactivePhysicsObject>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (base.hasAuthority)
                CheckMove();
        }

        /// <summary>
        /// Checks if the client wants to move.
        /// </summary>
        private void CheckMove()
        {
            Vector3 direction = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                0f,
                Input.GetAxisRaw("Vertical")
                );

            if (direction == Vector3.zero)
                return;

            //Move locally.
            ProcessInput(direction);
            _rpo.ReduceAggressiveness();

            //Only send inputs to server if not a client host.
            if (base.isClientOnly)
                CmdSendInput(direction);            
        }

        /// <summary>
        /// Applies an input direction to the rigidbody.
        /// </summary>
        /// <param name="input"></param>
        private void ProcessInput(Vector3 input)
        {
            //Add force first.
            input *= _directionalForce;
            //Add gravity to help keep the object dowm.
            input += Physics.gravity * 3f;

            //Apply to rigidbody.
            _rigidbody.AddForce(input, ForceMode.Force);
        }

        /// <summary>
        /// Tells the server which inputs to move.
        /// </summary>
        /// <param name="input"></param>
        [Command]
        private void CmdSendInput(Vector3 input)
        {
            ProcessInput(input);
        }
    }


}