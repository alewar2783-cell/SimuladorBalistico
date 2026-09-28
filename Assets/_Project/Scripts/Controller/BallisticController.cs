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
        private const float PROJECTILE_LIFETIME = 15f;

        [Header("References")]
        [SerializeField] private BallisticView _view;
        [SerializeField] private WeaponController _weapon;
        [SerializeField] private TargetSpawner _spawner;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private BallisticSim.Core.Repository.SimulationRepository _repository;

        private BallisticParameters _parameters = new BallisticParameters();
        private BallisticResults _results = new BallisticResults();
        private GameObject _activeProjectile;
        private Rigidbody _activeRb;
        private bool _isFiring;
        private bool _impactReceived;

        public BallisticParameters Parameters => _parameters;
        public BallisticResults Results => _results;

        private void OnEnable()
        {
            if (_view == null) return;
            
            _view.OnAngleChanged += v => { HandleAngleChanged(v); _cameraController.SetOverviewActive(); };
            _view.OnForceChanged += v => { _parameters.force = v; _cameraController.SetOverviewActive(); };
            _view.OnMassChanged += v => { _parameters.mass = v; _cameraController.SetOverviewActive(); };
            _view.OnBulletSizeChanged += v => { _parameters.bulletSize = v; _cameraController.SetOverviewActive(); };
            _view.OnBulletBouncinessChanged += v => { _parameters.bulletBounciness = v; _cameraController.SetOverviewActive(); };
            _view.OnDistanceChanged += v => HandleWallChange(() => _parameters.targetDistance = v);
            _view.OnWallColsChanged += v => HandleWallChange(() => _parameters.wallColumns = v);
            _view.OnWallRowsChanged += v => HandleWallChange(() => _parameters.wallRows = v);
            _view.OnBoxSizeChanged += v => HandleWallChange(() => _parameters.boxSize = v);
            _view.OnBoxMassChanged += v => HandleWallChange(() => _parameters.boxMass = v);
            _view.OnJointBreakForceChanged += v => HandleWallChange(() => _parameters.jointBreakForce = v);
            _view.OnFirePressed += HandleFire;
            _view.OnCleanScenePressed += HandleCleanScene;
            _view.OnExportDataPressed += HandleExportData;
            _view.OnShowHistoryPressed += ShowHistory;
        }

        private void OnDisable()
        {
            if (_view == null) return;
            
            _view.OnFirePressed -= HandleFire;
            _view.OnCleanScenePressed -= HandleCleanScene;
            _view.OnExportDataPressed -= HandleExportData;
            _view.OnShowHistoryPressed -= ShowHistory;
        }

        private void Start()
        {
            _view.HideReport();
            _view.ClearTelemetry();
            _view.SetCleanSceneButtonVisible(false);
            _weapon.SetAngle(_parameters.angle);
            
            UpdateSpawnerParams();
            _spawner.SpawnWall(_parameters.targetDistance);
        }

        private void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                HandleFire();
            }

            if (_activeRb == null) return;
            _view.UpdateTelemetry(_activeRb.linearVelocity.magnitude, _activeRb.transform.position);
        }

        private void HandleAngleChanged(float value)
        {
            _parameters.angle = value;
            _weapon.SetAngle(value);
        }

        private void UpdateSpawnerParams()
        {
            _spawner.SetWallParameters(_parameters.boxMass, _parameters.jointBreakForce, _parameters.wallColumns, _parameters.wallRows, _parameters.boxSize);
        }

        private void HandleWallChange(System.Action updateAction)
        {
            updateAction();
            UpdateSpawnerParams();
            if (!_isFiring) _spawner.SpawnWall(_parameters.targetDistance);
            
            Vector3 wallCenter = new Vector3(0f, (_parameters.wallRows * _parameters.boxSize) * 0.5f, _parameters.targetDistance);
            _cameraController.SetTargetActive(wallCenter);
        }

        private void HandleFire()
        {
            if (_isFiring) return;
            _isFiring = true;
            _impactReceived = false;

            _results.Reset();
            _view.HideReport();

            Transform spawn = _weapon.SpawnPoint;
            _activeProjectile = Instantiate(_projectilePrefab, spawn.position, spawn.rotation);

            var setup = _activeProjectile.GetComponent<ProjectileSetup>();
            if (setup == null)
            {
                Debug.LogError($"[{nameof(BallisticController)}] Projectile prefab missing ProjectileSetup.");
                HandleCleanScene();
                return;
            }

            setup.Configure(_parameters.mass, _parameters.bulletSize, _parameters.bulletBounciness);
            _activeRb = setup.Rb;
            _activeRb.AddForce(spawn.up * _parameters.force, ForceMode.Impulse);

            var tracking = _activeProjectile.GetComponent<ProjectileTracking>();
            if (tracking != null)
            {
                tracking.OnImpact += HandleImpact;
            }

            _view.SetFireButtonInteractable(false);
            _view.SetCleanSceneButtonVisible(true);
            _cameraController.FollowProjectile(_activeProjectile.transform);

            StartCoroutine(ProjectileLifetimeTimeout());
        }

        private IEnumerator ProjectileLifetimeTimeout()
        {
            yield return new WaitForSeconds(PROJECTILE_LIFETIME);
            if (!_impactReceived)
            {
                HandleCleanScene();
            }
        }

        private void HandleImpact(Vector3 impactPoint, float flightTime, float relativeVelocity, float impulse)
        {
            if (_impactReceived) return;
            _impactReceived = true;

            var tracking = _activeProjectile.GetComponent<ProjectileTracking>();
            _results.distance = tracking.GetDistance();
            _results.flightTime = flightTime;
            _results.impactPoint = impactPoint;
            _results.relativeVelocity = relativeVelocity;
            _results.collisionImpulse = impulse;
            
            _cameraController.SetImpactActive(impactPoint);

            StartCoroutine(ResolutionPhase());
        }

        private IEnumerator ResolutionPhase()
        {
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            
            yield return new WaitForSecondsRealtime(RESOLUTION_DELAY);
            
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

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

            SaveSimulationToHistory();
        }

        private void HandleCleanScene()
        {
            StopAllCoroutines();

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            if (_activeProjectile != null)
            {
                Destroy(_activeProjectile);
                _activeProjectile = null;
            }
            _activeRb = null;

            _isFiring = false;
            _impactReceived = false;
            _results.Reset();
            _spawner.SpawnWall(_parameters.targetDistance);
            _view.HideReport();
            _view.ClearTelemetry();
            _view.SetFireButtonInteractable(true);
            _view.SetCleanSceneButtonVisible(false);
            _cameraController.SetOverviewActive();
        }

        private void HandleExportData()
        {
            DataExporter.ExportToCsv(_parameters, _results);
        }

        private async void SaveSimulationToHistory()
        {
            if (_repository == null) return;
            _view.ShowCloudStatus("Guardando tiro en la nube...");
            
            try
            {
                bool isHit = _results.brokenJoints > 0;
                var record = new SimulationRecord(_parameters.angle, _parameters.force, _parameters.mass, _results.distance, isHit, _results.brokenJoints);
                await _repository.SaveRecordAsync(record);
                _view.ShowCloudStatus("Guardado Automático");
            }
            catch (System.Exception ex)
            {
                _view.ShowCloudStatus("Error al guardar");
                Debug.LogException(ex);
            }
        }

        private async void ShowHistory()
        {
            if (_repository == null) return;
            
            _view.ShowCloudStatus("Descargando Historial...");
            try
            {
                var history = await _repository.LoadHistoryAsync();
                _view.ShowCloudStatus("Historial Cargado");

                if (history.Count == 0)
                {
                    _view.ShowHistoryPanel("No hay tiros guardados aún.");
                    return;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                int startIdx = Mathf.Max(0, history.Count - 15);
                for (int i = startIdx; i < history.Count; i++)
                {
                    var r = history[i];
                    string hitTxt = r.hit ? "ACIERTO" : "FALLO";
                    sb.AppendLine($"[Tiro {i+1}] {hitTxt} | Ángulo: {r.angle:F1}° | Fuerza: {r.force:F0}N | Masa: {r.mass:F1}kg | Dist: {r.distance:F1}m | Afectados: {r.affectedObjects}");
                }
                _view.ShowHistoryPanel(sb.ToString());
            }
            catch (System.Exception ex)
            {
                _view.ShowCloudStatus("Error al cargar!");
                Debug.LogException(ex);
            }
        }
    }
}
