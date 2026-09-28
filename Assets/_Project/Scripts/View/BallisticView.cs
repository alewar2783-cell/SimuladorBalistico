using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BallisticSim.Core.View
{
    public class BallisticView : MonoBehaviour
    {
        [Header("Weapon Sliders")]
        [SerializeField] private Slider _angleSlider;
        [SerializeField] private Slider _forceSlider;
        [SerializeField] private Slider _massSlider;
        [SerializeField] private Slider _bulletSizeSlider;
        [SerializeField] private Slider _bulletBouncinessSlider;
        [SerializeField] private TMP_Text _angleLabelText;
        [SerializeField] private TMP_Text _forceLabelText;
        [SerializeField] private TMP_Text _massLabelText;
        [SerializeField] private TMP_Text _bulletSizeLabelText;
        [SerializeField] private TMP_Text _bulletBouncinessLabelText;

        [Header("Target Sliders")]
        [SerializeField] private Slider _distanceSlider;
        [SerializeField] private Slider _wallColsSlider;
        [SerializeField] private Slider _wallRowsSlider;
        [SerializeField] private Slider _boxSizeSlider;
        [SerializeField] private Slider _boxMassSlider;
        [SerializeField] private Slider _jointBreakForceSlider;
        [SerializeField] private TMP_Text _distanceLabelText;
        [SerializeField] private TMP_Text _wallColsLabelText;
        [SerializeField] private TMP_Text _wallRowsLabelText;
        [SerializeField] private TMP_Text _boxSizeLabelText;
        [SerializeField] private TMP_Text _boxMassLabelText;
        [SerializeField] private TMP_Text _jointBreakForceLabelText;

        [Header("Buttons & Panels")]
        [SerializeField] private Button _fireButton;
        [SerializeField] private TMP_Text _velocityText;
        [SerializeField] private TMP_Text _positionText;
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
        [SerializeField] private Button _showHistoryButton;
        [SerializeField] private TMP_Text _cloudStatusText;
        [SerializeField] private GameObject _historyPanel;
        [SerializeField] private TMP_Text _historyContentText;
        [SerializeField] private Button _closeHistoryButton;

        public event Action<float> OnAngleChanged;
        public event Action<float> OnForceChanged;
        public event Action<float> OnMassChanged;
        public event Action<float> OnBulletSizeChanged;
        public event Action<float> OnBulletBouncinessChanged;
        public event Action<float> OnDistanceChanged;
        public event Action<int> OnWallColsChanged;
        public event Action<int> OnWallRowsChanged;
        public event Action<float> OnBoxSizeChanged;
        public event Action<float> OnBoxMassChanged;
        public event Action<float> OnJointBreakForceChanged;
        public event Action OnFirePressed;
        public event Action OnCleanScenePressed;
        public event Action OnExportDataPressed;
        public event Action OnShowHistoryPressed;

        private void OnEnable()
        {
            if (_angleSlider == null) return;

            _angleSlider.onValueChanged.AddListener(v => { _angleLabelText.text = $"Ángulo: {v:F1}°"; OnAngleChanged?.Invoke(v); });
            _forceSlider.onValueChanged.AddListener(v => { _forceLabelText.text = $"Fuerza: {v:F0}N"; OnForceChanged?.Invoke(v); });
            _massSlider.onValueChanged.AddListener(v => { _massLabelText.text = $"Masa: {v:F2}kg"; OnMassChanged?.Invoke(v); });
            _bulletSizeSlider.onValueChanged.AddListener(v => { _bulletSizeLabelText.text = $"Tamaño: {v:F2}m"; OnBulletSizeChanged?.Invoke(v); });
            _bulletBouncinessSlider.onValueChanged.AddListener(v => { _bulletBouncinessLabelText.text = $"Rebote: {v:F2}"; OnBulletBouncinessChanged?.Invoke(v); });
            
            _distanceSlider.onValueChanged.AddListener(v => { _distanceLabelText.text = $"Distancia: {v:F0}m"; OnDistanceChanged?.Invoke(v); });
            _wallColsSlider.onValueChanged.AddListener(v => { _wallColsLabelText.text = $"Columnas: {v:F0}"; OnWallColsChanged?.Invoke(Mathf.RoundToInt(v)); });
            _wallRowsSlider.onValueChanged.AddListener(v => { _wallRowsLabelText.text = $"Filas: {v:F0}"; OnWallRowsChanged?.Invoke(Mathf.RoundToInt(v)); });
            _boxSizeSlider.onValueChanged.AddListener(v => { _boxSizeLabelText.text = $"Tam. Caja: {v:F2}m"; OnBoxSizeChanged?.Invoke(v); });
            _boxMassSlider.onValueChanged.AddListener(v => { _boxMassLabelText.text = $"Masa Caja: {v:F1}kg"; OnBoxMassChanged?.Invoke(v); });
            _jointBreakForceSlider.onValueChanged.AddListener(v => { _jointBreakForceLabelText.text = $"Fuerza Uniones: {v:F0}N"; OnJointBreakForceChanged?.Invoke(v); });

            _fireButton.onClick.AddListener(() => OnFirePressed?.Invoke());
            _cleanSceneButton.onClick.AddListener(() => OnCleanScenePressed?.Invoke());
            _exportDataButton.onClick.AddListener(() => OnExportDataPressed?.Invoke());
            if (_showHistoryButton != null) _showHistoryButton.onClick.AddListener(() => OnShowHistoryPressed?.Invoke());
            if (_closeHistoryButton != null) _closeHistoryButton.onClick.AddListener(() => HideHistoryPanel());
        }

        public void UpdateTelemetry(float velocityMagnitude, Vector3 position)
        {
            _velocityText.text = $"Velocidad: {velocityMagnitude:F2} m/s";
            _positionText.text = $"X:{position.x:F2}  Y:{position.y:F2}  Z:{position.z:F2}";
        }

        public void SetAngle(float angle)
        {
            if (_angleSlider != null) _angleSlider.value = angle;
        }

        public void SetForce(float force)
        {
            if (_forceSlider != null) _forceSlider.value = force;
        }

        public void ClearTelemetry()
        {
            _velocityText.text = "Velocidad: ---";
            _positionText.text = "X:---  Y:---  Z:---";
        }

        public void ShowReport(float distance, float flightTime, Vector3 impactPoint, float relativeVelocity, float collisionImpulse, int brokenJoints, int score)
        {
            _reportDistanceText.text = $"Distancia: {distance:F2}m";
            _reportFlightTimeText.text = $"Tiempo de Vuelo: {flightTime:F2}s";
            _reportImpactPointText.text = $"Impacto: ({impactPoint.x:F2}, {impactPoint.y:F2}, {impactPoint.z:F2})";
            _reportVelocityText.text = $"Vel. Relativa: {relativeVelocity:F2} m/s";
            _reportImpulseText.text = $"Impulso: {collisionImpulse:F2}";
            _reportBrokenJointsText.text = $"Piezas Rotas: {brokenJoints}";
            _reportScoreText.text = $"Puntaje: {score}";
            _reportPanel.SetActive(true);
        }

        public void HideReport() => _reportPanel.SetActive(false);
        public void SetFireButtonInteractable(bool interactable) => _fireButton.interactable = interactable;
        public void SetCleanSceneButtonVisible(bool isVisible) { if (_cleanSceneButton != null) _cleanSceneButton.gameObject.SetActive(isVisible); }

        public void ShowCloudStatus(string message)
        {
            if (_cloudStatusText != null)
                _cloudStatusText.text = message;
        }

        public void ShowHistoryPanel(string content)
        {
            if (_historyContentText != null) _historyContentText.text = content;
            if (_historyPanel != null) _historyPanel.SetActive(true);
        }

        public void HideHistoryPanel()
        {
            if (_historyPanel != null) _historyPanel.SetActive(false);
        }
    }
}
