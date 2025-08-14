using UnityEngine;

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

    [Header("PLAYERS")]
    [SerializeField] SpawnedCharacter[] playersPrefabs;

    private void Start()
    {
        //SpawnEnemies();
        //SpawnCharacters();
    }

    //private void SpawnEnemies()
    //{
    //    if (PhotonNetwork.IsMasterClient == false)
    //        return;

    //    foreach (SpawnedCharacter e in enemiesPrefabs)
    //    {
    //        GameObject _spawnedEnemy = PhotonNetwork.Instantiate(e.prefab.name, e.spawnPos.position, Quaternion.identity);
    //    }
    //}

    //private void SpawnCharacters()
    //{
    //    int _playerPrefabIndex;

    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        _playerPrefabIndex = 0;
    //    }
    //    else
    //    {
    //        _playerPrefabIndex = 1;
    //    }

    //    PhotonNetwork.Instantiate(playersPrefabs[_playerPrefabIndex].prefab.name, playersPrefabs[_playerPrefabIndex].spawnPos.position, Quaternion.identity);


    //}
}
