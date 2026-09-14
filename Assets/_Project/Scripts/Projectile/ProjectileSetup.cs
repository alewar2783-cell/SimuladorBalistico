using UnityEngine;

namespace BallisticSim.Core.Projectile
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class ProjectileSetup : MonoBehaviour
    {
        private Rigidbody _rb;

        public Rigidbody Rb => _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Configure(float mass, float size)
        {
            _rb.mass = mass;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.useGravity = true;

            float diameter = Mathf.Max(size, 0.05f);
            transform.localScale = Vector3.one * diameter;
        }
    }
}
