using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject player1Prefab; 
    [SerializeField] private GameObject player2Prefab;

    private void Start()
    {
        //NetworkManager.Singleton.StartHost();
    }
    //private void Update()
    //{

    //    if (Input.GetKeyDown(KeyCode.H))
    //        StartHost();

    //    if (Input.GetKeyDown(KeyCode.C))
    //        StartClient();
    //}

    public override void OnNetworkSpawn() //substitui Start ou Awake
    {
        //base.OnNetworkSpawn();
        Debug.Log("OnNetworkSpawn was called from " + this.name);

        if (IsServer)
        {
            Debug.Log("Im HOST! My ID is: " + NetworkManager.Singleton.LocalClientId);
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        }
        else
        {
            Debug.Log("Im CLIENT! My ID is" + NetworkManager.Singleton.LocalClientId);
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
        }
    }


    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId, int prefabId)
    {
        GameObject _newPlayer;
        if (prefabId == 0)
            _newPlayer = (GameObject)Instantiate(player1Prefab);
        else
            _newPlayer = (GameObject)Instantiate(player2Prefab);

        var _netObj = _newPlayer.GetComponent<NetworkObject>();
        _newPlayer.SetActive(true);
        _netObj.SpawnAsPlayerObject(clientId, true);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
