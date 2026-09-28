using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using BallisticSim.Core.Model;

namespace BallisticSim.Core.Repository
{
    public class UgsSimulationRepository : SimulationRepository
    {
        private const string HistoryPrefix = "simulation_";

        public override async Task SaveRecordAsync(SimulationRecord record)
        {
            string json = UnityEngine.JsonUtility.ToJson(record);
            string key = HistoryPrefix + record.id;

            var playerData = new Dictionary<string, object>
            {
                { key, json }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        }

        public override async Task<List<SimulationRecord>> LoadHistoryAsync()
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

            List<SimulationRecord> history = new List<SimulationRecord>();

            foreach (var item in playerData)
            {
                if (!item.Key.StartsWith(HistoryPrefix))
                {
                    continue;
                }

                try
                {
                    string json = item.Value.Value.GetAs<string>();
                    SimulationRecord record = UnityEngine.JsonUtility.FromJson<SimulationRecord>(json);
                    history.Add(record);
                }
                catch (System.Exception)
                {
                    // Ignore old or malformed keys (like our old 'simulation_history' which wasn't a string)
                    UnityEngine.Debug.LogWarning($"Skipping unreadable history key: {item.Key}");
                }
            }

            history.Sort((a, b) => string.Compare(a.timestamp, b.timestamp, System.StringComparison.Ordinal));
            return history;
        }
    }
}
