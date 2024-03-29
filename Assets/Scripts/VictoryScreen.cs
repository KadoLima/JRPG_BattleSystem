using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;

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

    private void OnEnable()
    {
        GameManager.OnGameWon += ShowScreen;
    }

    private void OnDisable()
    {
        GameManager.OnGameWon -= ShowScreen;

    }

    void Start()
    {
        restartButton.SetActive(false);
        quitButton.SetActive(false);

        restartButton.GetComponent<Button>().onClick.AddListener(GameManager.instance.RestartCurrentScene);
        quitButton.GetComponent<Button>().onClick.AddListener(GameManager.instance.QuitGame);
    }


    public void ShowScreen()
    {
        StartCoroutine(ShowScreenCoroutine());
        ShowXPEarned();
    }

    IEnumerator ShowScreenCoroutine()
    {
        yield return new WaitForSeconds(2.75f);
        screen.SetActive(true);


        int _createdItensAmount = 3;

        for (int i = 0; i < _createdItensAmount; i++)
        {
            int _randomIndex = Random.Range(0, foundItensPrefabs.Count);
            GameObject _randomLootItem = Instantiate(foundItensPrefabs[_randomIndex], foundItensContainer);
            foundItensPrefabs.RemoveAt(_randomIndex);

            CanvasGroup _itemCanvas = _randomLootItem.GetComponent<CanvasGroup>();

            _itemCanvas.alpha = 0;

            while (_itemCanvas.alpha < 1)
            {
                _itemCanvas.alpha += .1f;
                yield return null;
            }

            _randomLootItem.transform.DOScale(new Vector3(1.1f, 1.1f, 1), .2f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(.25f);
        }

        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            yield break;

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
        int _xpItensCreated = CombatManager.instance.playersOnField.Count;

        for (int i = 0; i < _xpItensCreated; i++)
        {
            GameObject g = Instantiate(playerXPItensPrefabs[i], playerXPItensContainer);
        }
    }

}
