using System;
using UnityEngine;

namespace BallisticSim.Core.Projectile
{
    public class ProjectileTracking : MonoBehaviour
    {
        public event Action<Vector3, float, float, float> OnImpact;

        private Vector3 _startPosition;
        private float _startTime;
        private bool _hasImpacted;

        private void Start()
        {
            _startPosition = transform.position;
            _startTime = Time.time;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hasImpacted) return;
            _hasImpacted = true;

            float flightTime = Time.time - _startTime;
            ContactPoint contact = collision.GetContact(0);
            float relativeVelocity = collision.relativeVelocity.magnitude;
            float impulse = collision.impulse.magnitude;

            OnImpact?.Invoke(contact.point, flightTime, relativeVelocity, impulse);
        }

        public float GetDistance()
        {
            return Vector3.Distance(_startPosition, transform.position);
        }
    }
}
