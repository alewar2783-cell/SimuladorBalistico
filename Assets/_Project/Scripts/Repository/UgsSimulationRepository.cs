using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using BallisticSim.Core.Model;

namespace BallisticSim.Core.Repository
{
    [System.Serializable]
    public class SimulationHistoryWrapper
    {
        public List<SimulationRecord> records = new List<SimulationRecord>();
    }

    public class UgsSimulationRepository : SimulationRepository
    {
        private const string HistoryKey = "simulation_history";
        private const int MaxHistorySize = 10;

        public override async Task SaveRecordAsync(SimulationRecord record)
        {
            var history = await LoadHistoryAsync();
            history.Add(record);
            
            if (history.Count > MaxHistorySize)
            {
                history.RemoveAt(0); // Keep last N records
            }

            var wrapper = new SimulationHistoryWrapper { records = history };
            var data = new Dictionary<string, object> { { HistoryKey, wrapper } };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }

        public override async Task<List<SimulationRecord>> LoadHistoryAsync()
        {
            var keys = new HashSet<string> { HistoryKey };
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (data.TryGetValue(HistoryKey, out var item))
            {
                var wrapper = item.Value.GetAs<SimulationHistoryWrapper>();
                if (wrapper != null && wrapper.records != null)
                {
                    return wrapper.records;
                }
            }

            return new List<SimulationRecord>();
        }
    }
}
