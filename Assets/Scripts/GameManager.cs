using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


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



    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EndGame()
    {
        OnGameWon?.Invoke();
    }
}
