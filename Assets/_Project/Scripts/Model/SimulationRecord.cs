using System;
using UnityEngine;

namespace BallisticSim.Core.Model
{
    [Serializable]
    public class SimulationRecord
    {
        public string id;
        public float angle;
        public float force;
        public float mass;
        public float distance;
        public bool hit;
        public int affectedObjects;
        public string timestamp;

        public SimulationRecord(float angle, float force, float mass, float distance, bool hit, int affectedObjects)
        {
            this.id = Guid.NewGuid().ToString("N");
            this.angle = angle;
            this.force = force;
            this.mass = mass;
            this.distance = distance;
            this.hit = hit;
            this.affectedObjects = affectedObjects;
            this.timestamp = DateTime.UtcNow.ToString("O");
        }
    }
}
