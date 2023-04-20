using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharactersSpawner : MonoBehaviour
{

    [System.Serializable]
    public struct SpawnedCharacter
    {
        public GameObject prefab;
        public Transform spawnPos;
    }

    [Header("ENEMIES")]
    [SerializeField] SpawnedCharacter[] enemiesPrefabs;
    //[SerializeField] Transform enemiesParent;

    [Header("PLAYERS")]
    [SerializeField] SpawnedCharacter[] playersPrefabs;
    //[SerializeField] Transform playersParent;


    private void Start()
    {
        SpawnEnemies();
        SpawnCharacters();
    }

    private void SpawnEnemies()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        foreach (SpawnedCharacter e in enemiesPrefabs)
        {
            GameObject _spawnedEnemy = PhotonNetwork.Instantiate(e.prefab.name, e.spawnPos.position, Quaternion.identity);
            //_spawnedEnemy.transform.SetParent(enemiesParent);
        }
    }

    private void SpawnCharacters()
    {
        int _playerPrefabIndex;

        if (PhotonNetwork.IsMasterClient)
        {
            _playerPrefabIndex = 0;
            //Debug.LogWarning("I'm master client. Spawning PREFAB_"+_playerPrefabIndex);
        }
        else
        {
            _playerPrefabIndex = 1;
            //Debug.LogWarning("I am not master client. Spawning PREFAB_" + _playerPrefabIndex);
        }

        PhotonNetwork.Instantiate(playersPrefabs[_playerPrefabIndex].prefab.name, playersPrefabs[_playerPrefabIndex].spawnPos.position, Quaternion.identity);


    }
}
