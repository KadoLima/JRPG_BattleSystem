using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    bool gameStarted;
    public bool GameStarted
    {
        get => gameStarted;
        set => gameStarted = value;
    }
    bool gameWon;
    public bool GameWon
    {
        get => gameWon;
    }

    public static GameManager instance;

    public static Action OnGameWon;
    public static Action OnGamePaused;
    public static Action OnGameResumed;
    public static Action OnPlayerDisconnectedFromGame;

    GameObject lastSelected;
    EventSystem eventSystem;

    [Header("DEBUG TOOLS")]
    [SerializeField] bool enemiesWontAttack;
    public bool EnemiesWontAttack => enemiesWontAttack;

    bool isPaused;
    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
    }

    void Awake()
    {
        instance = this;

        Time.timeScale = 1;

        eventSystem = EventSystem.current;

        ToggleDebugLog();
    }


    private void Start()
    {
        gameStarted = true;
    }

    private static void ToggleDebugLog()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
  Debug.unityLogger.logEnabled = false;
#endif
    }

    public void EndGame()
    {
        OnGameWon?.Invoke();
    }

    public void PauseGame()
    {
        if (!IsOnline() || IsServer)
        {
            lastSelected = eventSystem.currentSelectedGameObject;
            isPaused = true;
            OnGamePaused?.Invoke();
            Time.timeScale = 0;

            if (IsServer)
            {
                SyncPauseClientRpc(isPaused);
            }
        }
    }

    public void ResumeGame()
    {
        if (!IsOnline() || IsServer)
        {
            eventSystem.SetSelectedGameObject(lastSelected);
            isPaused = false;
            Time.timeScale = 1;
            OnGameResumed?.Invoke();

            if (IsServer)
            {
                SyncResumeClientRpc(isPaused);
            }
        }
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RestartCurrentScene()
    {
        if (IsOnline() && IsServer)
        {
            foreach (var netObj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.ToList())
            {
                if (netObj != NetworkManager.Singleton)
                {
                    netObj.Despawn(true);
                }
            }

            SyncRestartSceneClientRpc();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void QuitGame()
    {
        LoadMenuScene();
    }

    public static bool IsOnline() => NetworkManager.Singleton != null;

    #region ONLINE]


    [ClientRpc]
    private void SyncPauseClientRpc(bool isPausedValue)
    {
        isPaused = isPausedValue;
        OnGamePaused?.Invoke();
        Time.timeScale = 0;
    }

    [ClientRpc]
    void SyncResumeClientRpc(bool isPausedValue)
    {
        isPaused = isPausedValue;
        Time.timeScale = 1;
        OnGameResumed?.Invoke();
    }

    [ClientRpc]
    private void SyncRestartSceneClientRpc()
    {
        if (IsServer) return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
