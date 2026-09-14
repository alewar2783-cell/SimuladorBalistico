using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BallisticSim.Core.View
{
    public class BallisticView : MonoBehaviour
    {
        [Header("HUD — Input Sliders")]
        [SerializeField] private Slider _angleSlider;
        [SerializeField] private Slider _forceSlider;
        [SerializeField] private Slider _massSlider;
        [SerializeField] private Slider _bulletSizeSlider;
        [SerializeField] private Slider _distanceSlider;
        [SerializeField] private Slider _boxMassSlider;
        [SerializeField] private Slider _jointBreakForceSlider;

        [Header("HUD — Slider Labels")]
        [SerializeField] private TMP_Text _angleLabelText;
        [SerializeField] private TMP_Text _forceLabelText;
        [SerializeField] private TMP_Text _massLabelText;
        [SerializeField] private TMP_Text _bulletSizeLabelText;
        [SerializeField] private TMP_Text _distanceLabelText;
        [SerializeField] private TMP_Text _boxMassLabelText;
        [SerializeField] private TMP_Text _jointBreakForceLabelText;

        [Header("HUD — Fire Button")]
        [SerializeField] private Button _fireButton;

        [Header("Telemetry")]
        [SerializeField] private TMP_Text _velocityText;
        [SerializeField] private TMP_Text _positionText;

        [Header("Shot Report Panel")]
        [SerializeField] private GameObject _reportPanel;
        [SerializeField] private TMP_Text _reportDistanceText;
        [SerializeField] private TMP_Text _reportFlightTimeText;
        [SerializeField] private TMP_Text _reportImpactPointText;
        [SerializeField] private TMP_Text _reportVelocityText;
        [SerializeField] private TMP_Text _reportImpulseText;
        [SerializeField] private TMP_Text _reportBrokenJointsText;
        [SerializeField] private TMP_Text _reportScoreText;
        [SerializeField] private Button _cleanSceneButton;
        [SerializeField] private Button _exportDataButton;

        public event Action<float> OnAngleChanged;
        public event Action<float> OnForceChanged;
        public event Action<float> OnMassChanged;
        public event Action<float> OnBulletSizeChanged;
        public event Action<float> OnDistanceChanged;
        public event Action<float> OnBoxMassChanged;
        public event Action<float> OnJointBreakForceChanged;
        public event Action OnFirePressed;
        public event Action OnCleanScenePressed;
        public event Action OnExportDataPressed;

        private void OnEnable()
        {
            _angleSlider.onValueChanged.AddListener(HandleAngleChanged);
            _forceSlider.onValueChanged.AddListener(HandleForceChanged);
            _massSlider.onValueChanged.AddListener(HandleMassChanged);
            _bulletSizeSlider.onValueChanged.AddListener(HandleBulletSizeChanged);
            _distanceSlider.onValueChanged.AddListener(HandleDistanceChanged);
            _boxMassSlider.onValueChanged.AddListener(HandleBoxMassChanged);
            _jointBreakForceSlider.onValueChanged.AddListener(HandleJointBreakForceChanged);
            _fireButton.onClick.AddListener(HandleFirePressed);
            _cleanSceneButton.onClick.AddListener(HandleCleanScenePressed);
            _exportDataButton.onClick.AddListener(HandleExportDataPressed);
        }

        private void OnDisable()
        {
            _angleSlider.onValueChanged.RemoveListener(HandleAngleChanged);
            _forceSlider.onValueChanged.RemoveListener(HandleForceChanged);
            _massSlider.onValueChanged.RemoveListener(HandleMassChanged);
            _bulletSizeSlider.onValueChanged.RemoveListener(HandleBulletSizeChanged);
            _distanceSlider.onValueChanged.RemoveListener(HandleDistanceChanged);
            _boxMassSlider.onValueChanged.RemoveListener(HandleBoxMassChanged);
            _jointBreakForceSlider.onValueChanged.RemoveListener(HandleJointBreakForceChanged);
            _fireButton.onClick.RemoveListener(HandleFirePressed);
            _cleanSceneButton.onClick.RemoveListener(HandleCleanScenePressed);
            _exportDataButton.onClick.RemoveListener(HandleExportDataPressed);
        }

        private void HandleAngleChanged(float value)
        {
            _angleLabelText.text = $"Angle: {value:F1}°";
            OnAngleChanged?.Invoke(value);
        }

        private void HandleForceChanged(float value)
        {
            _forceLabelText.text = $"Force: {value:F0}N";
            OnForceChanged?.Invoke(value);
        }

        private void HandleMassChanged(float value)
        {
            _massLabelText.text = $"Mass: {value:F2}kg";
            OnMassChanged?.Invoke(value);
        }

        private void HandleBulletSizeChanged(float value)
        {
            _bulletSizeLabelText.text = $"Size: {value:F2}m";
            OnBulletSizeChanged?.Invoke(value);
        }

        private void HandleDistanceChanged(float value)
        {
            _distanceLabelText.text = $"Distance: {value:F0}m";
            OnDistanceChanged?.Invoke(value);
        }

        private void HandleBoxMassChanged(float value)
        {
            _boxMassLabelText.text = $"Box Mass: {value:F1}kg";
            OnBoxMassChanged?.Invoke(value);
        }

        private void HandleJointBreakForceChanged(float value)
        {
            _jointBreakForceLabelText.text = $"Joint Force: {value:F0}N";
            OnJointBreakForceChanged?.Invoke(value);
        }

        private void HandleFirePressed() => OnFirePressed?.Invoke();
        private void HandleCleanScenePressed() => OnCleanScenePressed?.Invoke();
        private void HandleExportDataPressed() => OnExportDataPressed?.Invoke();

        public void UpdateTelemetry(float velocityMagnitude, Vector3 position)
        {
            _velocityText.text = $"Speed: {velocityMagnitude:F2} m/s";
            _positionText.text = $"X:{position.x:F2}  Y:{position.y:F2}  Z:{position.z:F2}";
        }

        public void ClearTelemetry()
        {
            _velocityText.text = "Speed: ---";
            _positionText.text = "X:---  Y:---  Z:---";
        }

        public void ShowReport(
            float distance,
            float flightTime,
            Vector3 impactPoint,
            float relativeVelocity,
            float collisionImpulse,
            int brokenJoints,
            int score)
        {
            _reportDistanceText.text = $"Distance: {distance:F2}m";
            _reportFlightTimeText.text = $"Flight Time: {flightTime:F2}s";
            _reportImpactPointText.text = $"Impact: ({impactPoint.x:F2}, {impactPoint.y:F2}, {impactPoint.z:F2})";
            _reportVelocityText.text = $"Rel. Velocity: {relativeVelocity:F2} m/s";
            _reportImpulseText.text = $"Impulse: {collisionImpulse:F2}";
            _reportBrokenJointsText.text = $"Broken Joints: {brokenJoints}";
            _reportScoreText.text = $"Score: {score}";
            _reportPanel.SetActive(true);
        }

        public void HideReport()
        {
            _reportPanel.SetActive(false);
        }

        public void SetFireButtonInteractable(bool interactable)
        {
            _fireButton.interactable = interactable;
        }

        public void SetCleanSceneButtonVisible(bool isVisible)
        {
            if (_cleanSceneButton != null)
            {
                _cleanSceneButton.gameObject.SetActive(isVisible);
            }
        }
    }
}
