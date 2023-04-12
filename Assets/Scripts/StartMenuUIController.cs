using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuUIController : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] GameObject pressToStart;
    [SerializeField] Image fader;
    [SerializeField] CanvasGroup contentToFade;
    [SerializeField] ParticleSystem dustStorm;


    void Start()
    {
        fader.DOFade(0, 0);
        fader.gameObject.SetActive(false);
        pressToStart.SetActive(true);

        AssignButtons();
        HideButtons();
        PressToStart_Effect();

    }

    void Update()
    {
        if (Input.anyKeyDown && pressToStart.activeSelf)
        {
            pressToStart.SetActive(false);
            ShowButtons();
        }
        
    }

    void HideButtons()
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    void ShowButtons()
    {
        foreach (var b in buttons)
        {
            b.gameObject.SetActive(true);
        }

        EventSystem _eventSystem = EventSystem.current;
        _eventSystem.SetSelectedGameObject(buttons[0].gameObject);
    }

    void PressToStart_Effect()
    {
        TextMeshProUGUI _pressToStartText = pressToStart.GetComponent<TextMeshProUGUI>();

        Sequence _pulsingEffect = DOTween.Sequence();
        _pulsingEffect.Append(_pressToStartText.DOFade(1, .75f));
        _pulsingEffect.Append(_pressToStartText.DOFade(0, .75f));
        _pulsingEffect.SetLoops(-1);
    }

    void AssignButtons()
    {
        buttons[0].onClick.AddListener(delegate { FadeToDustAndLoadScene(1); });

        buttons[1].onClick.AddListener(ShowCoopMenu);
    }

    void FadeToDustAndLoadScene(int sceneIndex)
    {
        fader.gameObject.SetActive(true);
        Sequence fadeAndLoadSequence = DOTween.Sequence();
        fadeAndLoadSequence.Append(contentToFade.DOFade(0, .5f));
        dustStorm.Play();
        dustStorm.transform.DOMoveX(0, 3f).SetEase(Ease.OutQuint);
        fadeAndLoadSequence.AppendInterval(2.7f);
        fadeAndLoadSequence.OnComplete(() => SceneManager.LoadScene(1));
    }

    void ShowCoopMenu()
    {
        Debug.LogWarning("Showing coop menu");
    }
}
