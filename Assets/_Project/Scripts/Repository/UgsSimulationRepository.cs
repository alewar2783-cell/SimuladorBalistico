using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using BallisticSim.Core.Model;

namespace BallisticSim.Core.Repository
{
    public class UgsSimulationRepository : SimulationRepository
    {
        private const string LastSimulationKey = "last_simulation";

        public override async Task SaveAsync(SimulationRecord simulation)
        {
            var data = new Dictionary<string, object>
            {
                { LastSimulationKey, simulation }
            };

            await CloudSaveService.Instance
                .Data.Player
                .SaveAsync(data);
        }

        public override async Task<SimulationRecord> LoadLastAsync()
        {
            var keys = new HashSet<string>
            {
                LastSimulationKey
            };

            var data = await CloudSaveService.Instance
                .Data.Player
                .LoadAsync(keys);

            if (!data.TryGetValue(LastSimulationKey, out var item))
            {
                return null;
            }

            return item.Value.GetAs<SimulationRecord>();
        }
    }
}
