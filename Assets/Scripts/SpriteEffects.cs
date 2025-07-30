using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class SpriteEffects : MonoBehaviour
{
    [SerializeField] Color takeDamageColor;
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] Transform[] spotsToGetHit;

    SpriteRenderer mySpriteRenderer;
    Material myMaterial;
    Animator animator;
    int id;

    public Transform[] SpotsToGetHit => spotsToGetHit;

    public static Action<CharacterBehaviour> OnHit;

    private void OnEnable()
    {
        OnHit += HitEffect;
    }

    private void OnDisable()
    {
        OnHit -= HitEffect;
    }

    private void Start()
    {
        id = GetComponentInParent<CharacterBehaviour>().ID;

        mySpriteRenderer = GetComponent<SpriteRenderer>();

        myMaterial = GetComponent<SpriteRenderer>().material;

        animator = mySpriteRenderer.GetComponent<Animator>();
    }

    public void HitAction() //called as an Animation Event through the Animator
    {
        OnHit?.Invoke(GetComponentInParent<CharacterBehaviour>());
    }

    public void HitEffect(CharacterBehaviour attackingCharacter) 
    {
        CharacterBehaviour _player = attackingCharacter;

        if (_player.CurrentTarget != null)
        {

            if (_player.CurrentPreAction.isAreaOfEffect)
            {
                for (int i = 0; i < CombatManager.instance.EnemiesOnField.Count; i++)
                    CombatManager.instance.EnemiesOnField[i].GetComponentInChildren<SpriteEffects>().FlashRedAndPushBack();
            }
            else
                _player.CurrentTarget.GetComponentInChildren<SpriteEffects>().FlashRedAndPushBack();

            return;
        }

        var _enemyBehaviour = attackingCharacter.GetComponent<EnemyBehaviour>();

        if (_enemyBehaviour == null)
            return;

        if (_enemyBehaviour.CurrentPlayerTarget != null)
            _enemyBehaviour.CurrentPlayerTarget.GetComponentInChildren<SpriteEffects>().FlashRedAndPushBack();

    }

    void FlashRedAndPushBack() => StartCoroutine(FlashRedAndPullBack_Coroutine());

    IEnumerator FlashRedAndPullBack_Coroutine()
    {
        animator.SetTrigger("takeDamage");
        int _pushDirection = (mySpriteRenderer.transform.position.x <= 0) ? -2 : 2;

        Tweener _colorTween = mySpriteRenderer.DOColor(takeDamageColor, .1f);
        Tweener _moveTween = mySpriteRenderer.transform.DOLocalMoveX(.2f * _pushDirection, .2f);

        yield return new WaitForSeconds(.2f);

        _colorTween = mySpriteRenderer.DOColor(Color.white, .1f);
        _moveTween = mySpriteRenderer.transform.DOLocalMoveX(.2f * -_pushDirection, .2f);

        yield return _moveTween.WaitForCompletion();
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
