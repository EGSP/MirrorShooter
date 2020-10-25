using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.ReactivePhyics.Demos
{

    public class Controlled2D : NetworkBehaviour
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
        private Rigidbody2D _rigidbody2D;
        #endregion

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (base.isClient)
                CheckMove();
        }

        /// <summary>
        /// Checks if the client wants to move.
        /// </summary>
        private void CheckMove()
        {
            if (!base.hasAuthority)
                return;

            Vector3 direction = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                0f
                );

            if (direction == Vector3.zero)
                return;

            //Move locally.
            ProcessInput(direction);

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
            input += Physics.gravity;

            //Apply to rigidbody.
            _rigidbody2D.AddForce(input, ForceMode2D.Force);
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