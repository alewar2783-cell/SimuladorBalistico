using System;
using UnityEngine;

namespace BallisticSim.Core.Model
{
    [Serializable]
    public class SimulationRecord
    {
        public float angle;
        public float force;
        public float distance;
        public string timestamp;

        public SimulationRecord(float angle, float force, float distance)
        {
            this.angle = angle;
            this.force = force;
            this.distance = distance;
            this.timestamp = DateTime.UtcNow.ToString("O");
        }
    }
}
