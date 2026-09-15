using System.Threading.Tasks;
using UnityEngine;
using BallisticSim.Core.Model;

namespace BallisticSim.Core.Repository
{
    public abstract class SimulationRepository : MonoBehaviour
    {
        public abstract Task SaveRecordAsync(SimulationRecord record);
        public abstract Task<System.Collections.Generic.List<SimulationRecord>> LoadHistoryAsync();
    }
}
