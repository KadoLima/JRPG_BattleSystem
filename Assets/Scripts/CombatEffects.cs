using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CombatEffects : MonoBehaviour
{
    SpriteRenderer mySpriteRenderer;
    [SerializeField] Color takeDamageColor;
    [SerializeField] SpriteRenderer shadow;
    Material myMaterial;

    private void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        myMaterial = GetComponent<SpriteRenderer>().material;
    }
    public void ShakeCamera()
    {
        ScreenEffects.instance.ShakeCamera();
        CharacterBehaviour player = GetComponentInParent<CharacterBehaviour>();

        if (player.CurrentEnemy != null)
        {
            if (player.CurrentPreAction.isAreaOfEffect)
            {
                for (int i = 0; i < CombatManager.instance.enemiesOnField.Count; i++)
                {
                    CombatManager.instance.enemiesOnField[i].GetComponentInChildren<CombatEffects>().FlashRed();
                }
            }
            else
            {
                player.CurrentEnemy.GetComponentInChildren<CombatEffects>().FlashRed();
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
        float x = this.transform.position.x;
        mySpriteRenderer.DOColor(takeDamageColor, .1f);

        int pushDirection;
        if (mySpriteRenderer.transform.position.x < 0)
            pushDirection = -1;
        else pushDirection = 1;

        mySpriteRenderer.transform.DOLocalMoveX(.2f * pushDirection, .2f);
        yield return new WaitForSeconds(.2f);
        mySpriteRenderer.DOColor(Color.white, .1f);
        mySpriteRenderer.transform.DOLocalMoveX(.2f * -pushDirection, .2f);
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
            dissolveAmount += Time.fixedDeltaTime/30;
            myMaterial.SetFloat("_FadeAmount", dissolveAmount);
            yield return null;
        }
    }

    IEnumerator FadeOutShadowCoroutine()
    {
        float counter = shadow.color.a;
        while (counter>0)
        {
            counter -= Time.fixedDeltaTime/20;
            shadow.color = new Color(0, 0, 0, counter);
            yield return null;
        }
    }

}
