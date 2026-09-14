using System.Collections;
using UnityEngine;
using BallisticSim.Core.Model;
using BallisticSim.Core.View;
using BallisticSim.Core.Projectile;
using BallisticSim.Core.Spawner;
using BallisticSim.Core.Data;
using BallisticSim.Core.Camera;

namespace BallisticSim.Core.Controllers
{
    public class BallisticController : MonoBehaviour
    {
        private const float RESOLUTION_DELAY = 3f;

        [Header("References")]
        [SerializeField] private BallisticView _view;
        [SerializeField] private WeaponController _weapon;
        [SerializeField] private TargetSpawner _spawner;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private CameraController _cameraController;

        private BallisticParameters _parameters = new BallisticParameters();
        private BallisticResults _results = new BallisticResults();
        private GameObject _activeProjectile;
        private bool _isFiring;

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
            _view.OnCleanScenePressed += HandleCleanScene;
            _view.OnExportDataPressed += HandleExportData;
        }

        private void OnDisable()
        {
            _view.OnAngleChanged -= HandleAngleChanged;
            _view.OnForceChanged -= HandleForceChanged;
            _view.OnMassChanged -= HandleMassChanged;
            _view.OnBulletSizeChanged -= HandleBulletSizeChanged;
            _view.OnDistanceChanged -= HandleDistanceChanged;
            _view.OnFirePressed -= HandleFire;
            _view.OnCleanScenePressed -= HandleCleanScene;
            _view.OnExportDataPressed -= HandleExportData;
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
            if (_activeProjectile == null) return;

            var rb = _activeProjectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                _view.UpdateTelemetry(rb.linearVelocity.magnitude, _activeProjectile.transform.position);
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
            if (_isFiring) return;
            _isFiring = true;

            _results.Reset();
            _view.HideReport();

            Transform spawn = _weapon.SpawnPoint;
            _activeProjectile = Instantiate(_projectilePrefab, spawn.position, spawn.rotation);

            var setup = _activeProjectile.GetComponent<ProjectileSetup>();
            if (setup == null)
            {
                Debug.LogError($"[{nameof(BallisticController)}] Projectile prefab missing ProjectileSetup.");
                return;
            }

            setup.Configure(_parameters.mass, _parameters.bulletSize);
            setup.Rb.AddForce(spawn.up * _parameters.force, ForceMode.Impulse);

            var tracking = _activeProjectile.GetComponent<ProjectileTracking>();
            if (tracking != null)
            {
                tracking.OnImpact += HandleImpact;
            }

            _view.SetFireButtonInteractable(false);
            _cameraController.FollowProjectile(_activeProjectile.transform);
        }

        private void HandleImpact(Vector3 impactPoint, float flightTime, float relativeVelocity, float impulse)
        {
            var tracking = _activeProjectile.GetComponent<ProjectileTracking>();
            _results.distance = tracking.GetDistance();
            _results.flightTime = flightTime;
            _results.impactPoint = impactPoint;
            _results.relativeVelocity = relativeVelocity;
            _results.collisionImpulse = impulse;

            StartCoroutine(ResolutionPhase());
        }

        private IEnumerator ResolutionPhase()
        {
            yield return new WaitForSeconds(RESOLUTION_DELAY);

            _results.brokenJoints = _spawner.CountBrokenJoints();
            int score = _results.brokenJoints;

            _view.ShowReport(
                _results.distance,
                _results.flightTime,
                _results.impactPoint,
                _results.relativeVelocity,
                _results.collisionImpulse,
                _results.brokenJoints,
                score
            );
        }

        private void HandleCleanScene()
        {
            StopAllCoroutines();

            if (_activeProjectile != null)
            {
                Destroy(_activeProjectile);
                _activeProjectile = null;
            }

            _isFiring = false;
            _results.Reset();
            _spawner.SpawnWall(_parameters.targetDistance);
            _view.HideReport();
            _view.ClearTelemetry();
            _view.SetFireButtonInteractable(true);
            _cameraController.SetOverviewActive();
        }

        private void HandleExportData()
        {
            DataExporter.ExportToCsv(_parameters, _results);
        }
    }
}
