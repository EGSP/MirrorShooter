using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.FlexNetworkTransforms.Demos
{

    public class AuthMotor : NetworkBehaviour
    {
        [SerializeField]
        private float _moveRate = 3f;

        private void FixedUpdate()
        {
            if (!base.hasAuthority)
                return;

            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal == 0f)
                return;

            CmdRunInput(horizontal);
        }

        [Command]
        private void CmdRunInput(float horizontal)
        {
            Move(horizontal);
        }

        private void Move(float horizontal)
        {
            if (horizontal == 0f)
                return;

            transform.position += new Vector3(horizontal * _moveRate * Time.deltaTime, 0f, 0f);

            //Switch facing.
            if (horizontal > 0f)
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
            else
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f, transform.eulerAngles.z);
        }



    }


}