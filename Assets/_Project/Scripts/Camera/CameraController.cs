using UnityEngine;
using Unity.Cinemachine;

namespace BallisticSim.Core.Camera
{
    public class CameraController : MonoBehaviour
    {
        private const int PRIORITY_ACTIVE = 20;
        private const int PRIORITY_INACTIVE = 10;

        [SerializeField] private CinemachineCamera _overviewCamera;
        [SerializeField] private CinemachineCamera _targetCamera;
        [SerializeField] private CinemachineCamera _trackingCamera;
        [SerializeField] private CinemachineCamera _impactCamera;

        private void Start()
        {
            SetOverviewActive();
        }

        public void SetOverviewActive()
        {
            _overviewCamera.Priority = PRIORITY_ACTIVE;
            if (_targetCamera != null) _targetCamera.Priority = PRIORITY_INACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        public void SetTargetActive(Vector3 wallCenter)
        {
            if (_targetCamera != null)
            {
                // Place camera looking at the wall from an angle
                _targetCamera.transform.position = new Vector3(-8f, Mathf.Max(wallCenter.y + 2f, 4f), wallCenter.z - 8f);
                _targetCamera.Target.LookAtTarget = null; // Clear tracking target if any
                _targetCamera.transform.LookAt(wallCenter);
                
                _targetCamera.Priority = PRIORITY_ACTIVE;
            }
            
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        public void FollowProjectile(Transform target)
        {
            _trackingCamera.Target.TrackingTarget = target;
            _trackingCamera.Target.LookAtTarget = target;
            _trackingCamera.Priority = PRIORITY_ACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            if (_targetCamera != null) _targetCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        public void SetImpactActive(Transform impactCenter)
        {
            if (_impactCamera == null) return;
            
            // Position camera to the right and slightly back from the impact point
            _impactCamera.transform.position = new Vector3(8f, 4f, impactCenter.position.z - 2f);
            _impactCamera.Target.LookAtTarget = impactCenter;
            
            _impactCamera.Priority = PRIORITY_ACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            if (_targetCamera != null) _targetCamera.Priority = PRIORITY_INACTIVE;
        }
    }
}
