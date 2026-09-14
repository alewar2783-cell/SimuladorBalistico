using UnityEngine;
using BallisticSim.Core.Model;
using BallisticSim.Core.View;
using BallisticSim.Core.Projectile;
using BallisticSim.Core.Spawner;

namespace BallisticSim.Core.Controllers
{
    public class BallisticController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BallisticView _view;
        [SerializeField] private WeaponController _weapon;
        [SerializeField] private TargetSpawner _spawner;
        [SerializeField] private GameObject _projectilePrefab;

        private BallisticParameters _parameters = new BallisticParameters();
        private BallisticResults _results = new BallisticResults();
        private GameObject _activeProjectile;

        public BallisticParameters Parameters => _parameters;
        public BallisticResults Results => _results;

        private void OnEnable()
        {
            _view.OnAngleChanged += HandleAngleChanged;
            _view.OnForceChanged += HandleForceChanged;
            _view.OnMassChanged += HandleMassChanged;
            _view.OnBulletSizeChanged += HandleBulletSizeChanged;
            _view.OnDistanceChanged += HandleDistanceChanged;
            _view.OnFirePressed += HandleFire;
        }

        private void OnDisable()
        {
            _view.OnAngleChanged -= HandleAngleChanged;
            _view.OnForceChanged -= HandleForceChanged;
            _view.OnMassChanged -= HandleMassChanged;
            _view.OnBulletSizeChanged -= HandleBulletSizeChanged;
            _view.OnDistanceChanged -= HandleDistanceChanged;
            _view.OnFirePressed -= HandleFire;
        }

        private void Start()
        {
            _view.HideReport();
            _view.ClearTelemetry();
            _weapon.SetAngle(_parameters.angle);
            _spawner.SpawnWall(_parameters.targetDistance);
        }

        private void Update()
        {
            if (_activeProjectile != null)
            {
                var rb = _activeProjectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    _view.UpdateTelemetry(rb.linearVelocity.magnitude, _activeProjectile.transform.position);
                }
            }
        }

        private void HandleAngleChanged(float value)
        {
            _parameters.angle = value;
            _weapon.SetAngle(value);
        }

        private void HandleForceChanged(float value) => _parameters.force = value;
        private void HandleMassChanged(float value) => _parameters.mass = value;
        private void HandleBulletSizeChanged(float value) => _parameters.bulletSize = value;
        private void HandleDistanceChanged(float value)
        {
            _parameters.targetDistance = value;
            _spawner.SpawnWall(value);
        }

        private void HandleFire()
        {
            if (_activeProjectile != null) return;

            _results.Reset();
            _view.HideReport();

            Transform spawn = _weapon.SpawnPoint;
            _activeProjectile = Instantiate(_projectilePrefab, spawn.position, spawn.rotation);

            var setup = _activeProjectile.GetComponent<ProjectileSetup>();
            if (setup != null)
            {
                setup.Configure(_parameters.mass, _parameters.bulletSize);
                setup.Rb.AddForce(spawn.up * _parameters.force, ForceMode.Impulse);
            }

            _view.SetFireButtonInteractable(false);
        }

        public void DestroyProjectile()
        {
            if (_activeProjectile != null)
            {
                Destroy(_activeProjectile);
                _activeProjectile = null;
            }
            _view.SetFireButtonInteractable(true);
            _view.ClearTelemetry();
        }
    }
}
