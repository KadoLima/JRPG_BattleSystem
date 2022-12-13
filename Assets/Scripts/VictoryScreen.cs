using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VictoryScreen : MonoBehaviour
{
    [Header("FOUND ITENS")]
    [SerializeField] List<GameObject> foundItensPrefabs = new List<GameObject>();
    [SerializeField] Transform foundItensContainer;
    [Header("XP EARNED")]
    [SerializeField] GameObject[] playerXPItensPrefabs;
    [SerializeField] Transform playerXPItensContainer;

    [SerializeField] GameObject screen;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    void ShowXPEarned()
    {
        int xpItensCreated = CombatManager.instance.playersOnField.Count;

        for (int i = 0; i < xpItensCreated; i++)
        {
            GameObject g = Instantiate(playerXPItensPrefabs[i], playerXPItensContainer);
        }
    }



}
