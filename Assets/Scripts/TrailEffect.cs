using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrailEffect : MonoBehaviour
{
    float timeBetweenSpawns;
    [SerializeField] float startTimeBetweenSpawns;
    [SerializeField] GameObject echo;

    SpriteRenderer mySpriteRenderer;
    CharacterBehaviour characterBehaviour;

    bool canShowTrail = false;

    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        characterBehaviour = GetComponentInParent<CharacterBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canShowTrail)
            return;

        if (timeBetweenSpawns <= 0)
        {

            GameObject g = Instantiate(echo, transform.position, Quaternion.identity);

            SpriteRenderer _gSpriteRenderer = g.GetComponentInChildren<SpriteRenderer>();
            _gSpriteRenderer.sprite = mySpriteRenderer.sprite;
            _gSpriteRenderer.DOColor(new Color(1, 1, 1, 0), .2f);
            Destroy(g, 1f);
            timeBetweenSpawns = startTimeBetweenSpawns;
        }
        else
        {
            timeBetweenSpawns -= Time.deltaTime;
        }
    }

    public void ShowTrail()
    {
        canShowTrail = true;
    }

    public void HideTrail()
    {
        canShowTrail = false;
    }

    //public void ShowTrailEffect()
    //{
    //    StartCoroutine(ShowTrailEffect_Coroutine(.5f));
    //}

    //IEnumerator ShowTrailEffect_Coroutine(float periodOfTime)
    //{
    //    while (periodOfTime >0)
    //    {
    //        GameObject g = Instantiate(echo, transform.position, Quaternion.identity);
    //        g.GetComponentInChildren<SpriteRenderer>().DOColor(new Color(1, 1, 1, 0), .2f);
    //        Destroy(g, 1f);
    //        yield return new WaitForSeconds(timeBetweenSpawns);
    //        periodOfTime -= Time.deltaTime;
    //    }
    //}
}
