using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CombatEffects : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer;
    [SerializeField] Color takeDamageColor;
    [SerializeField] SpriteRenderer shadow;

    [SerializeField] Transform[] spotsToGetHit;

    public Transform[] SpotsToGetHit => spotsToGetHit;

    Material myMaterial;

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        myMaterial = GetComponent<SpriteRenderer>().material;
    }
    public void ShakeCamera()
    {
        ScreenEffects.instance.ShakeCamera();
        CharacterBehaviour _player = GetComponentInParent<CharacterBehaviour>();

        if (_player.CurrentTarget != null)
        {
            if (_player.CurrentPreAction.isAreaOfEffect)
            {
                for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
                {
                    CombatManager.instance.enemiesOnField[i].GetComponentInChildren<CombatEffects>().FlashRed();
                }
            }
            else
            {
                _player.CurrentTarget.GetComponentInChildren<CombatEffects>().FlashRed();
            }
            return;
        }

        if (GetComponentInParent<EnemyBehaviour>().CurrentPlayerTarget != null)
            GetComponentInParent<EnemyBehaviour>().CurrentPlayerTarget.GetComponentInChildren<CombatEffects>().FlashRed();

    }

    public void FlashRed()
    {
        StartCoroutine(FlashRedCoroutine());
    }

    IEnumerator FlashRedCoroutine()
    {
        //float x = this.transform.position.x;
        mySpriteRenderer.DOColor(takeDamageColor, .1f);

        int _pushDirection;
        if (mySpriteRenderer.transform.position.x < 0)
            _pushDirection = -1;
        else _pushDirection = 1;

        mySpriteRenderer.transform.DOLocalMoveX(.2f * _pushDirection, .2f);
        yield return new WaitForSeconds(.2f);
        mySpriteRenderer.DOColor(Color.white, .1f);
        mySpriteRenderer.transform.DOLocalMoveX(.2f * -_pushDirection, .2f);
    }

    public void DieEffect()
    {
        StartCoroutine(DissolveCoroutine());
        StartCoroutine(FadeOutShadowCoroutine());
    }

    IEnumerator DissolveCoroutine()
    {
        myMaterial.SetFloat("_FadeAmount",0);
        float dissolveAmount = 0;
        while (dissolveAmount < 1)
        {
            dissolveAmount += Time.deltaTime/5;
            myMaterial.SetFloat("_FadeAmount", dissolveAmount);
            yield return null;
        }
    }

    IEnumerator FadeOutShadowCoroutine()
    {
        float _counter = shadow.color.a;
        while (_counter > 0)
        {
            _counter -= Time.fixedDeltaTime / 20;
            shadow.color = new Color(0, 0, 0, _counter);
            yield return null;
        }
    }

}
