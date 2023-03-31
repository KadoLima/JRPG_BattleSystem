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

    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        characterBehaviour = GetComponentInParent<CharacterBehaviour>();
    }

    void Update()
    {
        if (!canShowTrail)
            return;

        if (timeBetweenSpawns <= 0)
        {

            GameObject _echo = Instantiate(echo, transform.position, Quaternion.identity);

            SpriteRenderer _gSpriteRenderer = _echo.GetComponentInChildren<SpriteRenderer>();
            _gSpriteRenderer.sprite = mySpriteRenderer.sprite;
            _gSpriteRenderer.DOColor(new Color(1, 1, 1, 0), .2f);
            Destroy(_echo, 1f);
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
}
