using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class ScreenEffects : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] SpriteRenderer skillDarkScreen;
    [SerializeField] Image critEffect;

    void Awake()
    {
        skillDarkScreen.color = new Color(0, 0, 0, 0);

        SpriteEffects.OnHit += ShakeCamera;
        SpriteEffects.OnHit += WhiteBlink;

        critEffect.color = new Color(1, 1, 1, 0);
    }

    private void OnEnable()
    {
        CharacterBehaviour.OnSkillUsed += ShowDarkScreen;
        CharacterBehaviour.OnSkillEnded += HideDarkScreen;

        EnemyBehaviour.OnEnemyUsedSkill += ShowDarkScreen;
    }

    private void OnDisable()
    {
        CharacterBehaviour.OnSkillUsed -= ShowDarkScreen;
        CharacterBehaviour.OnSkillEnded -= HideDarkScreen;

        EnemyBehaviour.OnEnemyUsedSkill -= ShowDarkScreen;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void ShakeCamera(CharacterBehaviour c)
    {

        if (c.IsDoingCritDamageAction)
            mainCamera.DOShakePosition(.3f, .8f, 50);
        else mainCamera.DOShakePosition(.3f, .4f, 50);
    }

    public void ShowDarkScreen(string s)
    {
        //Debug.LogWarning("dark screen effect activated");
        skillDarkScreen.DOColor(new Color(0, 0, 0, .6f), .25f);
    }

    public void HideDarkScreen()
    {
        skillDarkScreen.DOColor(new Color(0, 0, 0, 0), .25f);
    }

    public void WhiteBlink(CharacterBehaviour c)
    {
        if (c == null || c.IsDoingCritDamageAction == false)
            return;

        Sequence _sequence = DOTween.Sequence();
        _sequence.Append(critEffect.DOFade(.8f, .1f));
        _sequence.Append(critEffect.DOFade(0, .05f));
    }

}
