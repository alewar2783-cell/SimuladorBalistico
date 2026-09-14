using UnityEngine;
using BallisticSim.Core.Model;
using BallisticSim.Core.View;

namespace BallisticSim.Core.Controllers
{
    public class BallisticController : MonoBehaviour
    {
        [SerializeField] private BallisticView _view;

        private BallisticParameters _parameters = new BallisticParameters();
        private BallisticResults _results = new BallisticResults();

        public BallisticParameters Parameters => _parameters;
        public BallisticResults Results => _results;

        private void OnEnable()
        {
            _view.OnAngleChanged += HandleAngleChanged;
            _view.OnForceChanged += HandleForceChanged;
            _view.OnMassChanged += HandleMassChanged;
            _view.OnBulletSizeChanged += HandleBulletSizeChanged;
            _view.OnDistanceChanged += HandleDistanceChanged;
        }

        private void OnDisable()
        {
            _view.OnAngleChanged -= HandleAngleChanged;
            _view.OnForceChanged -= HandleForceChanged;
            _view.OnMassChanged -= HandleMassChanged;
            _view.OnBulletSizeChanged -= HandleBulletSizeChanged;
            _view.OnDistanceChanged -= HandleDistanceChanged;
        }

        private void Start()
        {
            _view.HideReport();
            _view.ClearTelemetry();
        }

        private void HandleAngleChanged(float value) => _parameters.angle = value;
        private void HandleForceChanged(float value) => _parameters.force = value;
        private void HandleMassChanged(float value) => _parameters.mass = value;
        private void HandleBulletSizeChanged(float value) => _parameters.bulletSize = value;
        private void HandleDistanceChanged(float value) => _parameters.targetDistance = value;
    }
}
