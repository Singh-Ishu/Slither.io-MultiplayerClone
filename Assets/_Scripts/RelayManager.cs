using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }
    public string JoinCode { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized successfully.");

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in anonymously successfully.");
            }

            // Start server only after Unity Services + Auth are ready
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                Debug.Log("Running in headless mode - starting server.");
                StartServerWithRelay();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize Unity Services or authenticate: {e.Message}");
        }
    }


    public async void StartServerWithRelay(int maxConnections = 10)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("Cannot start server: Not signed in to Authentication Service.");
                return;
            }
            Debug.Log("Authentication verified, proceeding with Relay allocation.");

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            Debug.Log("Allocation created successfully.");

            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code: {JoinCode}");

            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (utp == null)
            {
                Debug.LogError("UnityTransport component not found on NetworkManager!");
                return;
            }

            utp.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            bool started = NetworkManager.Singleton.StartServer();
            if (!started)
            {
                Debug.LogError("Failed to start server.");
            }
            else
            {
                Debug.Log("Server started successfully.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start server with Relay: {e.Message}");
        }
    }

    public async void JoinWithRelay(string joinCode)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("Cannot join: Not signed in to Authentication Service.");
                return;
            }

            if (string.IsNullOrEmpty(joinCode))
            {
                Debug.LogError("Join code is null or empty!");
                return;
            }

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (utp == null)
            {
                Debug.LogError("UnityTransport component not found on NetworkManager!");
                return;
            }

            utp.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData
            );

            bool started = NetworkManager.Singleton.StartClient();
            if (!started)
            {
                Debug.LogError("Failed to start client.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join with Relay: {e.Message}");
        }
    }

    public async void StartHostWithRelay(int maxConnections = 10)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogError("Cannot start host: Not signed in to Authentication Service.");
                return;
            }

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code: {JoinCode}");

            UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (utp == null)
            {
                Debug.LogError("UnityTransport component not found on NetworkManager!");
                return;
            }

            utp.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            bool started = NetworkManager.Singleton.StartHost();
            if (!started)
            {
                Debug.LogError("Failed to start host.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start host with Relay: {e.Message}");
        }
    }
}