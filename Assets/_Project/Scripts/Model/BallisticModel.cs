using System;
using UnityEngine;

namespace BallisticSim.Core.Model
{
    [Serializable]
    public class BallisticParameters
    {
        [Range(0f, 90f)]
        public float angle = 45f;
        public float force = 500f;
        public float mass = 1f;
        public float bulletSize = 0.2f;
        public float targetDistance = 30f;
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
