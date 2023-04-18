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

    //[Header("GAME MODES")]
    //[SerializeField] bool isOnlineCoop;
    //public bool IsOnlineCoop => isOnlineCoop;

    [Header("DEBUG TOOLS")]
    [SerializeField] bool enemiesWontAttack;
    public bool EnemiesWontAttack => enemiesWontAttack;



    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameStarted = true;
    }

    public void EndGame()
    {
        OnGameWon?.Invoke();
    }

}
