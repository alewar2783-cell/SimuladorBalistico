using System;
using UnityEngine;

namespace BallisticSim.Core.Model
{
    [Serializable]
    public class BallisticParameters
    {
        [Header("Weapon & Projectile")]
        [Range(0f, 90f)]
        public float angle = 5f;
        public float force = 50f;
        public float mass = 1f;
        public float bulletSize = 0.2f;
        public float bulletBounciness = 0.3f;

        [Header("Target Parameters")]
        public float targetDistance = 30f;
        public int wallColumns = 5;
        public int wallRows = 5;
        public float boxSize = 1f;
        public float boxMass = 1f;
        public float jointBreakForce = 50f;
    }

    [Serializable]
    public class BallisticResults
    {
        public float distance;
        public float flightTime;
        public Vector3 impactPoint;
        public float relativeVelocity;
        public float collisionImpulse;
        public int brokenJoints;

        public void Reset()
        {
            distance = 0f;
            flightTime = 0f;
            impactPoint = Vector3.zero;
            relativeVelocity = 0f;
            collisionImpulse = 0f;
            brokenJoints = 0;
        }
    }
}
