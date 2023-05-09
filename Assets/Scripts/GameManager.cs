using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
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
    PhotonView myPhotonView;

    //[Header("GAME MODES")]
    //[SerializeField] bool isOnlineCoop;
    //public bool IsOnlineCoop => isOnlineCoop;

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
        myPhotonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        gameStarted = true;

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
        
    }

    public void EndGame()
    {
        OnGameWon?.Invoke();
    }

    public void PauseGame()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            lastSelected = eventSystem.currentSelectedGameObject;
            isPaused = true;
            OnGamePaused?.Invoke();
            Time.timeScale = 0;

            if (PhotonNetwork.IsConnected)
            {
                myPhotonView.RPC(nameof(SyncPause), RpcTarget.Others, isPaused);
            }

        }

    }

    public void ResumeGame()
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            eventSystem.SetSelectedGameObject(lastSelected);
            isPaused = false;
            Time.timeScale = 1;
            OnGameResumed?.Invoke();

            if (PhotonNetwork.IsConnected)
            {
                myPhotonView.RPC(nameof(SyncResume), RpcTarget.Others, isPaused);
                //PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = -1;
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
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            myPhotonView.RPC(nameof(SyncRestartScene), RpcTarget.Others);
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void QuitGame()
    {
        //if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        //{
        //    myPhotonView.RPC(nameof(SyncQuitGame), RpcTarget.Others);
        //   //mostrar o aviso que o player desconectou. O load da cena inicial só deve acontecer depois do jogador apertar o OK nesse aviso.
        //}


        LoadMenuScene();
    }

    [PunRPC]
    void SyncPause(bool isPausedValue)
    {
        Debug.LogWarning("pausing P2");
        isPaused = isPausedValue;
        OnGamePaused?.Invoke();
        Time.timeScale = 0;
    }

    [PunRPC]
    void SyncResume(bool isPausedValue)
    {
        isPaused = isPausedValue;
        Time.timeScale = 1;
        Debug.LogWarning("resuming P2");
        OnGameResumed?.Invoke();
    }

    [PunRPC]
    void SyncRestartScene()
    {
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogWarning("PLAYER LEFT ROOM!");
        Time.timeScale = 0;
        OnPlayerDisconnectedFromGame?.Invoke();
    }


}
