using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject player1Prefab; // Estelle - Host
    [SerializeField] private GameObject player2Prefab; // Vivi - Client

    private bool hasSpawnedHost = false;
    private bool hasSpawnedClient = false;

    private void Update()
    {
        TestSpawning();
    }

    private void TestSpawning()
    {
        if (Input.GetKeyDown(KeyCode.T) && NetworkManager.Singleton != null && !NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("Starting as Host...");
            NetworkManager.Singleton.StartHost();
        }

        if (Input.GetKeyDown(KeyCode.C) && NetworkManager.Singleton != null && !NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsClient)
        {
            Debug.LogWarning("Starting as Client...");
            NetworkManager.Singleton.StartClient();
        }
    }

    private void Awake()
    {
        StartCoroutine(WaitForNetworkManagerAndSubscribe());
    }

    private IEnumerator WaitForNetworkManagerAndSubscribe()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);

        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleServerStarted()
    {
        if (!hasSpawnedHost && IsServer)
        {
            Debug.LogWarning("Server started - Spawning player 1 (Host)");
            SpawnPlayer(NetworkManager.ServerClientId, player1Prefab);
            hasSpawnedHost = true;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (IsServer && clientId != NetworkManager.ServerClientId && !hasSpawnedClient)
        {
            Debug.LogWarning($"Client connected: {clientId} - Spawning player 2 (Client)");
            SpawnPlayer(clientId, player2Prefab);
            hasSpawnedClient = true;
        }
    }

    private void SpawnPlayer(ulong clientId, GameObject prefab)
    {
        if (!IsServer)
        {
            Debug.LogWarning("Only server can spawn players!");
            return;
        }

        if (prefab == null)
        {
            Debug.LogError("Player prefab is null! Assign it in the inspector.");
            return;
        }

        GameObject playerInstance = Instantiate(prefab);
        NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("Player prefab must have a NetworkObject.");
            Destroy(playerInstance);
            return;
        }

        playerInstance.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);

        Debug.LogWarning($"Spawned {prefab.name} for clientId {clientId}");
    }
}