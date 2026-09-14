using UnityEngine;
using Unity.Cinemachine;

namespace BallisticSim.Core.Camera
{
    public class CameraController : MonoBehaviour
    {
        private const int PRIORITY_ACTIVE = 20;
        private const int PRIORITY_INACTIVE = 10;

        [SerializeField] private CinemachineCamera _overviewCamera;
        [SerializeField] private CinemachineCamera _trackingCamera;
        [SerializeField] private CinemachineCamera _impactCamera;

        private void Start()
        {
            SetOverviewActive();
        }

        public void SetOverviewActive()
        {
            _overviewCamera.Priority = PRIORITY_ACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        public void FollowProjectile(Transform target)
        {
            _trackingCamera.Follow = target;
            _trackingCamera.LookAt = target;
            _trackingCamera.Priority = PRIORITY_ACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        public void SetImpactActive(Transform impactCenter)
        {
            if (_impactCamera == null) return;
            
            // Position camera to the right and slightly back from the impact point
            _impactCamera.transform.position = new Vector3(8f, 4f, impactCenter.position.z - 2f);
            _impactCamera.LookAt = impactCenter;
            
            _impactCamera.Priority = PRIORITY_ACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
        }
    }
}
