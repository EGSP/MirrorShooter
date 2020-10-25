using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.NetworkProximities.Demos
{


    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField]
        private bool _networkSpawn = false;
        [SerializeField]
        private int _spawnCount = 100;
        [SerializeField]
        private GameObject _rollableCubePrefab;
        [SerializeField]
        private Vector3 _originArea;

        private void Awake()
        {
            if (!NetworkServer.active && _networkSpawn)
                return;

            SpawnRollableCubes();
        }


        private void SpawnRollableCubes()
        {
            for (int i = 0; i < _spawnCount; i++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-_originArea.x, _originArea.x),
                    Random.Range(-_originArea.y, _originArea.y),
                    Random.Range(-_originArea.z, _originArea.z)
                    );

                GameObject result = Instantiate(_rollableCubePrefab, transform.position + offset, Quaternion.identity);
                if (_networkSpawn)
                    NetworkServer.Spawn(result);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, _originArea);
        }
    }


}