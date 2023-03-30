using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScreenEffects : MonoBehaviour
{
    //public static ScreenEffects instance;
    Camera mainCamera;
    [SerializeField] SpriteRenderer skillDarkScreen;

    void Awake()
    {
        //instance = this;

        skillDarkScreen.color = new Color(0, 0, 0, 0);

        SpriteEffects.OnHit += ShakeCamera;
    }

    private void OnEnable()
    {
        CharacterBehaviour.OnUsedSkill += ShowDarkScreen;
        CharacterBehaviour.OnSkillEnded += HideDarkScreen;
    }

    private void OnDisable()
    {
        CharacterBehaviour.OnUsedSkill -= ShowDarkScreen;
        CharacterBehaviour.OnSkillEnded -= HideDarkScreen;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    public void ShakeCamera(int a)
    {
        //Debug.LogWarning("SHAKE SHAKE!");
        mainCamera.DOShakePosition(.35f, .4f, 50);
    }

    public void ShowDarkScreen()
    {
        skillDarkScreen.DOColor(new Color(0, 0, 0, .6f), .25f);
    }

    public void HideDarkScreen()
    {
        skillDarkScreen.DOColor(new Color(0, 0, 0, 0), .25f);
    }

}
