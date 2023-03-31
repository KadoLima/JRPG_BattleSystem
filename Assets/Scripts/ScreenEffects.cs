using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenEffects : MonoBehaviour
{
    Camera mainCamera;
    [SerializeField] SpriteRenderer skillDarkScreen;

    void Awake()
    {
        skillDarkScreen.color = new Color(0, 0, 0, 0);

        SpriteEffects.OnHit += ShakeCamera;
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

    public void ShakeCamera(int a)
    {
        mainCamera.DOShakePosition(.35f, .4f, 50);
    }

    public void ShowDarkScreen(string s)
    {
        skillDarkScreen.DOColor(new Color(0, 0, 0, .6f), .25f);
    }

    public void HideDarkScreen()
    {
        skillDarkScreen.DOColor(new Color(0, 0, 0, 0), .25f);
    }

}
