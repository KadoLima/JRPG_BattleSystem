using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class CharacterUIController : MonoBehaviour
{
    [SerializeField] CanvasGroup myCanvasGroup;
    [SerializeField] GameObject pointer;
    [SerializeField] BattlePanel battlePanel;
    [SerializeField] Transform techsPanel;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI floatingText;
    [SerializeField] TextMeshProUGUI skillDescText;
    float originalFloatingTextY;
    [field: SerializeField] public Image cooldownBar { get; private set; }

    CharacterBehaviour characterBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        pointer.SetActive(false);

        if (cooldownBar)
            cooldownBar.fillAmount = 0;

        characterBehaviour = GetComponent<CharacterBehaviour>();

        if (battlePanel)
            battlePanel.gameObject.SetActive(false);

        RefreshHP(characterBehaviour.CurrentHP, characterBehaviour.CurrentHP);

        ResetFloatingText();

        SetSkillNames();
    }

    private void ResetFloatingText()
    {
        floatingText.text = "";

        originalFloatingTextY = floatingText.rectTransform.anchoredPosition.y;
    }

    public void ShowCanvas()
    {
        StartCoroutine(ShowMyCanvasCoroutine());
    }

    public void HideCanvas(float speed=10, float delay=0)
    {
        StartCoroutine(HideMyCanvasCoroutine(speed,delay));
    }

    IEnumerator ShowMyCanvasCoroutine()
    {
        while (myCanvasGroup.alpha < 1)
        {
            myCanvasGroup.alpha += Time.deltaTime * 10f;
            yield return null;
        }

        myCanvasGroup.alpha = 1;
    }

    IEnumerator HideMyCanvasCoroutine(float speed=10, float delay=0)
    {
        yield return new WaitForSeconds(delay);
        while (myCanvasGroup.alpha > 0)
        {
            myCanvasGroup.alpha -= Time.deltaTime * speed;
            yield return null;
        }

        myCanvasGroup.alpha = 0;
    }

     public void ShowBattlePanel()
    {
        battlePanel.gameObject.SetActive(true);
    }

    public void HideBattlePanel()
    {
        battlePanel.gameObject.SetActive(false);
        techsPanel.gameObject.SetActive(false);
    }

    public void ShowHidePointer(bool s)
    {
        pointer.SetActive(s);
    }

    public void RefreshHP(int currentHP, int baseHP)
    {

        if (currentHP > 0)
            hpText.text = currentHP + "/" + baseHP;
        else hpText.text = 0 + "/" + baseHP;
    }

    public void ShowFloatingDamageText(int damageAmount, bool isHealing = false)
    {
        StartCoroutine(FloatingTextCoroutine(damageAmount, isHealing));
    }

    IEnumerator FloatingTextCoroutine(int damageAmount, bool isHealing = false)
    {
        float _popMovingTime = .2f;
        float _fadeTime = .2f;
        float yMovingAmount = 40f;
        float _showingTime = _popMovingTime + 1f;

        Color _finalColor;

        if (isHealing)
            _finalColor = Color.green;
        else
            _finalColor = Color.white;

        floatingText.text = damageAmount.ToString();
        floatingText.color = new Color(1, 1, 1, 0);
        floatingText.DOColor(_finalColor, _fadeTime);
        floatingText.rectTransform.DOAnchorPosY(floatingText.rectTransform.anchoredPosition.y + yMovingAmount, _popMovingTime).OnComplete(BounceFloatingText);
        yield return new WaitForSeconds(_showingTime);
        floatingText.DOColor(new Color(0, 0, 0, 0), _fadeTime);
        //yield return new WaitForSeconds(_fadeTime);
        //floatingText.rectTransform.localPosition = floatingTextPos;
    }

    void BounceFloatingText()
    {
        floatingText.rectTransform.DOAnchorPosY(originalFloatingTextY, .4f).SetEase(Ease.OutBounce);
    }

    public void UpdateSkillDescText(string t)
    {
        skillDescText.text = t;
    }

    public void SetSkillNames()
    {
        if (!techsPanel)
            return;

        for (int i = 0; i < characterBehaviour.Skills.Length; i++)
        {
            if (techsPanel.GetChild(0).GetChild(i).gameObject.activeSelf)
                techsPanel.GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>().text = characterBehaviour.Skills[i].actionName;
        }
    }
}
