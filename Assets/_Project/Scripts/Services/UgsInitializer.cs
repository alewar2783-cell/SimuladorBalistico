using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace BallisticSim.Core.Services
{
    public class UgsInitializer : MonoBehaviour
    {
        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                Debug.Log("[UGS] Ready. Player ID: " + AuthenticationService.Instance.PlayerId);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[UGS] Initialization Error: {ex.Message}");
            }
        }
    }
}
