using UnityEngine;

namespace BallisticSim.Core.Controllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public Transform SpawnPoint => _spawnPoint;

        public void SetAngle(float angleDegrees)
        {
            // 0 degrees = horizontal (towards +Z). That means rotating +90 on X so local Y points to +Z.
            // 90 degrees = vertical (towards +Y). That means 0 rotation on X.
            transform.localRotation = Quaternion.Euler(90f - angleDegrees, 0f, 0f);
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
