using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] Transform gameOverPanel;
    [SerializeField] GameObject firstSelected;

    void Start()
    {
        screen.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        screen.GetComponent<CanvasGroup>().alpha = 0;
        screen.SetActive(true);
        StartCoroutine(ShowScreenCoroutine());
        gameOverPanel.GetComponent<RectTransform>().DOAnchorPosY(0, .2f);
    }

    IEnumerator ShowScreenCoroutine()
    {
        CanvasGroup _canvasGroup = screen.GetComponent<CanvasGroup>();
        EventSystem _eventSystem = EventSystem.current;

        _eventSystem.SetSelectedGameObject(firstSelected);

        while (_canvasGroup.alpha < 1)
        {
            _canvasGroup.alpha += Time.deltaTime * 5f;
            yield return null;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
