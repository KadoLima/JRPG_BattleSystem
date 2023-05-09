using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;

public class PauseOverlay : MonoBehaviour
{
    [Header("Screen content")]
    [SerializeField] GameObject screen;
    CanvasGroup screenCanvasGroup;

    [Header("Buttons")]
    [SerializeField] GameObject buttonsParent;
    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;

    [Header("Message Player2")]
    [SerializeField] GameObject notHostMessage;
    EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        SetFirstSelected();
        GameManager.OnGamePaused += ShowPauseOverlay;
        GameManager.OnGameResumed += HidePauseOverlay;
        GameManager.OnPlayerDisconnectedFromGame += HideButtons;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= ShowPauseOverlay;
        GameManager.OnGameResumed -= HidePauseOverlay;
        GameManager.OnPlayerDisconnectedFromGame -= HideButtons;
    }

    private void Start()
    {
        AssignButtons();
        screenCanvasGroup = screen.GetComponent<CanvasGroup>();
        HidePauseOverlay();
    }

    void AssignButtons()
    {
        resumeButton.onClick.AddListener(GameManager.instance.ResumeGame);
        restartButton.onClick.AddListener(GameManager.instance.RestartCurrentScene);
        quitButton.onClick.AddListener(GameManager.instance.QuitGame);
    }

    public void ShowPauseOverlay()
    {
        Debug.LogWarning("SHOWING PAUSE OVERLAY");

        screenCanvasGroup.alpha = 0;
        screen.SetActive(true);
        SetFirstSelected();

        screenCanvasGroup.DOFade(1, .15f).SetEase(Ease.Linear).SetUpdate(true);

        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            buttonsParent.SetActive(true);
        else
            buttonsParent.SetActive(false);

        notHostMessage.SetActive(!buttonsParent.activeSelf);
    }

    public void HidePauseOverlay()
    {
        screenCanvasGroup.DOFade(0, 0f).SetUpdate(true).OnComplete(() => screen.SetActive(false));
    }

    public void HideButtons()
    {
        buttonsParent.SetActive(false);
    }

    private void SetFirstSelected() => eventSystem.SetSelectedGameObject(resumeButton.gameObject);
    
}
