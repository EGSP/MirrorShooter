using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.FlexNetworkTransforms.Demos
{

    public class AutoMotor : NetworkBehaviour
    {
        public float MoveRate = 2f;

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(__Move());
        }

        private IEnumerator __Move()
        {
            Vector3[] steps = new Vector3[] {
                transform.position,
                transform.position + new Vector3(4f, 0f,0f),
                transform.position + new Vector3(3f, 0f,0f),
                transform.position + new Vector3(8f, 0f,0f),
                transform.position
                };

            int step = 0;
            while (true)
            {
                if (transform.position == steps[step])
                    step++;
                //Reset.
                if (step == steps.Length)
                { 
                    step = 0;
                    yield return new WaitForSeconds(1f);
                }

                transform.position = Vector3.MoveTowards(transform.position, steps[step], MoveRate * Time.deltaTime);

                yield return null;
            }
        }

    }


}