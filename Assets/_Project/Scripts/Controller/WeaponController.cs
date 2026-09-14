using UnityEngine;

namespace BallisticSim.Core.Controllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public Transform SpawnPoint => _spawnPoint;

        public void SetAngle(float angleDegrees)
        {
            transform.localRotation = Quaternion.Euler(angleDegrees, 0f, 0f);
        }

        private void OnValidate()
        {
            if (_spawnPoint == null)
            {
                Debug.LogWarning($"[{nameof(WeaponController)}] SpawnPoint is not assigned.", this);
            }
        }
    }
}
