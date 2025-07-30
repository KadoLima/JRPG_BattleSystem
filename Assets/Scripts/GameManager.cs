using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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
  Debug.logger.logEnabled = false;
#endif
    }

    public void EndGame()
    {
        OnGameWon?.Invoke();
    }

    public void PauseGame()
    {
        lastSelected = eventSystem.currentSelectedGameObject;
        isPaused = true;
        OnGamePaused?.Invoke();
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        eventSystem.SetSelectedGameObject(lastSelected);
        isPaused = false;
        Time.timeScale = 1;
        OnGameResumed?.Invoke();
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        LoadMenuScene();
    }
}
