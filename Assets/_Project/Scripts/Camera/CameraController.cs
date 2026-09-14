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

        private void Start()
        {
            SetOverviewActive();
        }

        public void SetOverviewActive()
        {
            _overviewCamera.Priority = PRIORITY_ACTIVE;
            _trackingCamera.Priority = PRIORITY_INACTIVE;
        }

        public void FollowProjectile(Transform target)
        {
            _trackingCamera.Follow = target;
            _trackingCamera.LookAt = target;
            _trackingCamera.Priority = PRIORITY_ACTIVE;
            _overviewCamera.Priority = PRIORITY_INACTIVE;
        }
    }
}
