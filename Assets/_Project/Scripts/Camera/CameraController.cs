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
                var tgt = _targetCamera.Target;
                tgt.LookAtTarget = null; // Clear tracking target if any
                _targetCamera.Target = tgt;
                _targetCamera.transform.LookAt(wallCenter);
                
                _targetCamera.Priority = PRIORITY_ACTIVE;
            }
            
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        public void FollowProjectile(Transform target)
        {
            var tgt = _trackingCamera.Target;
            tgt.TrackingTarget = target;
            tgt.LookAtTarget = target;
            _trackingCamera.Target = tgt;
            _trackingCamera.Priority = PRIORITY_ACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            if (_targetCamera != null) _targetCamera.Priority = PRIORITY_INACTIVE;
            if (_impactCamera != null) _impactCamera.Priority = PRIORITY_INACTIVE;
        }

        private Transform _impactTargetHelper;

        public void SetImpactActive(Vector3 impactPoint)
        {
            if (_impactCamera == null) return;
            
            if (_impactTargetHelper == null)
            {
                _impactTargetHelper = new GameObject("ImpactTargetHelper").transform;
            }
            _impactTargetHelper.position = impactPoint;

            // Position camera to the right and slightly back from the impact point
            _impactCamera.transform.position = new Vector3(8f, 4f, impactPoint.z - 2f);
            
            var tgt = _impactCamera.Target;
            tgt.LookAtTarget = _impactTargetHelper;
            _impactCamera.Target = tgt;
            
            _impactCamera.Priority = PRIORITY_ACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
            if (_targetCamera != null) _targetCamera.Priority = PRIORITY_INACTIVE;
        }
    }
}
