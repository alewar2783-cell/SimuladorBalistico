using System.Threading.Tasks;
using UnityEngine;
using BallisticSim.Core.Model;

namespace BallisticSim.Core.Repository
{
    public abstract class SimulationRepository : MonoBehaviour
    {
        public abstract Task SaveAsync(SimulationRecord simulation);
        public abstract Task<SimulationRecord> LoadLastAsync();
    }
}
