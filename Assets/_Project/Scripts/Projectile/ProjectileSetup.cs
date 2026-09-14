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

        public void Configure(float mass, float size, float bounciness)
        {
            if (Rb != null)
            {
                Rb.mass = mass;
                Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                Rb.useGravity = true;
            }

            float diameter = Mathf.Max(size, 0.05f);
            transform.localScale = Vector3.one * diameter;

            var col = GetComponent<Collider>();
            if (col != null)
            {
                PhysicsMaterial mat = new PhysicsMaterial("BulletPhysics");
                mat.bounciness = bounciness;
                mat.bounceCombine = PhysicsMaterialCombine.Maximum;
                col.sharedMaterial = mat;
            }
        }
    }
}
