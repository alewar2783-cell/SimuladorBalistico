using UnityEditor;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using BallisticSim.Core.Model;
using BallisticSim.Core.Repository;
using System.Threading.Tasks;

namespace BallisticSim.EditorTests
{
    public static class UGSTester
    {
        [MenuItem("BallisticSim/Run UGS Cloud Test")]
        public static async void RunFromEditor()
        {
            Debug.Log("[UGS_TEST] Starting UGS Test from Editor Menu...");
            await RunAsync();
            Debug.Log("[UGS_TEST] SUCCESS: Editor UGS Test completed.");
        }

        public static void RunFromCLI()
        {
            Debug.Log("[UGS_TEST] Starting UGS Test from CLI...");
            
            // To run async code in batchmode and wait for it, we register to EditorApplication.update
            Task testTask = RunAsync();
            
            EditorApplication.update += () => 
            {
                if (testTask.IsCompleted)
                {
                    if (testTask.IsFaulted)
                    {
                        Debug.LogError($"[UGS_TEST] FAILED: {testTask.Exception}");
                        EditorApplication.Exit(1);
                    }
                    else
                    {
                        Debug.Log("[UGS_TEST] SUCCESS: UGS operations completed properly.");
                        EditorApplication.Exit(0);
                    }
                }
            };
        }

        private static async Task RunAsync()
        {
            Debug.Log("[UGS_TEST] Initializing Unity Services...");
            await UnityServices.InitializeAsync();
            
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("[UGS_TEST] Signing in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            
            Debug.Log($"[UGS_TEST] Authenticated. PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Create temporary GameObject for the Repository
            var go = new GameObject("TempRepo");
            var repo = go.AddComponent<UgsSimulationRepository>();

            Debug.Log("[UGS_TEST] Creating a mock Simulation Record...");
            var record = new SimulationRecord(angle: 35f, force: 900f, mass: 2.5f, distance: 45.2f, hit: true, affectedObjects: 8);
            
            Debug.Log($"[UGS_TEST] Saving record with ID: {record.id}");
            await repo.SaveRecordAsync(record);
            
            Debug.Log("[UGS_TEST] Fetching history from Cloud Save...");
            var history = await repo.LoadHistoryAsync();
            
            Debug.Log($"[UGS_TEST] Found {history.Count} records in the cloud.");
            foreach(var r in history)
            {
                Debug.Log($"[UGS_TEST] -> ID: {r.id} | Angle: {r.angle}° | Force: {r.force}N | Hit: {r.hit}");
            }
            
            Object.DestroyImmediate(go);
        }
    }
}
