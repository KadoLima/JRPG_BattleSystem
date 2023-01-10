using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class VictoryScreen : MonoBehaviour
{
    [Header("FOUND ITENS")]
    [SerializeField] List<GameObject> foundItensPrefabs = new List<GameObject>();
    [SerializeField] Transform foundItensContainer;
    [Header("XP EARNED")]
    [SerializeField] GameObject[] playerXPItensPrefabs;
    [SerializeField] Transform playerXPItensContainer;

    [SerializeField] GameObject screen;

    [Header("BUTTONS")]
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject quitButton;



    // Start is called before the first frame update
    void Start()
    {
        restartButton.SetActive(false);
        quitButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ShowScreen();
        }
    }


    public void ShowScreen()
    {
        screen.SetActive(true);

        StartCoroutine(ShowFoundItensCoroutine());
        ShowXPEarned();
    }

    IEnumerator ShowFoundItensCoroutine()
    {
        int createdItensAmount = 3;

        for (int i = 0; i < createdItensAmount; i++)
        {
            int randomIndex = Random.Range(0, foundItensPrefabs.Count);
            GameObject g = Instantiate(foundItensPrefabs[randomIndex], foundItensContainer);
            foundItensPrefabs.RemoveAt(randomIndex);

            CanvasGroup itemCanvas = g.GetComponent<CanvasGroup>();

            itemCanvas.alpha = 0;

            while (itemCanvas.alpha < 1)
            {
                itemCanvas.alpha += .1f;
                yield return null;
            }

            g.transform.DOScale(new Vector3(1.1f, 1.1f, 1), .2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(.25f);
        }

        yield return new WaitForSeconds(0.5f);

        restartButton.GetComponent<CanvasGroup>().alpha = 0;
        restartButton.SetActive(true);
        quitButton.GetComponent<CanvasGroup>().alpha = 0;
        quitButton.SetActive(true);
        restartButton.GetComponent<CanvasGroup>().DOFade(1, .25f);
        quitButton.GetComponent<CanvasGroup>().DOFade(1, .25f);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }

    void ShowXPEarned()
    {
        int xpItensCreated = CombatManager.instance.playersOnField.Count;

        for (int i = 0; i < xpItensCreated; i++)
        {
            GameObject g = Instantiate(playerXPItensPrefabs[i], playerXPItensContainer);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }



}
