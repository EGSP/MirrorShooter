using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.NetworkProximities.Demos
{


    public class AutoMovePlayer : NetworkBehaviour
    {
        public float _moveRate = 22f;

        private float[] _ranges;

        private bool _movingLeft = true;

        private void Awake()
        {
            this.enabled = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _ranges = new float[] { transform.position.z - 50f, transform.position.z + 50f };
            this.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            float x = (_movingLeft) ? _ranges[0] : _ranges[1];
            Vector3 goal = new Vector3(x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, goal, _moveRate * Time.deltaTime);
            if (transform.position == goal)
                _movingLeft = !_movingLeft;
        }
    }

}
