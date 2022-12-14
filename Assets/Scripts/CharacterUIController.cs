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
    //[SerializeField] Transform techsPanel;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;
    [Header("Floating Combat Text")]
    [SerializeField] TextMeshProUGUI floatingText;
    [SerializeField] Color healColor;
    [SerializeField] Color manaColor;
    [Space(10)]
    [SerializeField] TextMeshProUGUI descriptionTooltipText;
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


        Invoke(nameof(RefreshHPMP), .1f);

        ResetFloatingText();

        //SetSkillNames();
    }

    public void RefreshHPMP()
    {
        RefreshHP(characterBehaviour.CurrentHP, characterBehaviour.MyStats.baseHP);
        RefreshMP(characterBehaviour.CurrentMP, characterBehaviour.MyStats.baseMP);
    }

    private void ResetFloatingText()
    {
        floatingText.text = "";

        originalFloatingTextY = floatingText.rectTransform.anchoredPosition.y;
    }

    //public void ShowCanvas()
    //{
    //    StartCoroutine(ShowMyCanvasCoroutine());
    //}

    //IEnumerator ShowMyCanvasCoroutine()
    //{
    //    while (myCanvasGroup.alpha < 1)
    //    {
    //        myCanvasGroup.alpha += Time.deltaTime * 10f;
    //        yield return null;
    //    }

    //    myCanvasGroup.alpha = 1;
    //}

    public void HideCanvas(float speed = 10, float delay = 0)
    {
        StartCoroutine(HideMyCanvasCoroutine(speed, delay));
    }

    IEnumerator HideMyCanvasCoroutine(float speed=10, float delay=0)
    {
        yield return new WaitForSeconds(delay);
        while (myCanvasGroup.alpha > 0)
        {
            myCanvasGroup.alpha -= Time.deltaTime * speed;
            yield return null;
        }

        HideBattlePanel();

        myCanvasGroup.alpha = 0;
    }

    public BattlePanel GetBattlePanel()
    {
        if (!battlePanel)
            return null;

        return battlePanel;
    }

     public void ShowBattlePanel()
    {
        if (!battlePanel)
            return;

        battlePanel.gameObject.SetActive(true);
        battlePanel.SetFirstSelected();
        HideDescriptionTooltip();
    }

    public void HideBattlePanel()
    {
        if (!battlePanel)
            return;

        battlePanel.HideSubPanels();
        battlePanel.gameObject.SetActive(false);
    }

    public void ShowHidePointer(bool s)
    {
        pointer.SetActive(s);
    }

    public void HideUI()
    {
        hpText.DOColor(new Color(0, 0, 0, 0f), .2f);

        if (mpText)
            mpText.DOColor(new Color(0, 0, 0, 0f), .2f);

        if (cooldownBar)
            cooldownBar.GetComponentInParent<CanvasGroup>().alpha = 0;
    }

    public void ShowUI()
    {
        hpText.DOColor(new Color(0, 0, 0, 1f), .2f);

        if (mpText)
            mpText.DOColor(new Color(0, 0, 0, 1f), .2f);

        if (cooldownBar)
            cooldownBar.GetComponentInParent<CanvasGroup>().alpha = 1;
    }

    public void RefreshHP(int currentHP, int baseHP)
    {

        if (currentHP > 0)
            hpText.text = "H: " + currentHP + "/" + baseHP;
        else hpText.text = "H: " + 0 + "/" + baseHP;
    }

    public void RefreshMP(int currentMP, int baseMP)
    {
        if (mpText == null)
            return;

        if (currentMP > 0)
            mpText.text = "M: " + currentMP + "/" + baseMP;
        else mpText.text = "M: " + 0 + "/" + baseMP;
    }

    public void ShowFloatingDamageText(int damageAmount, DamageType dmgType)
    {
        //Debug.LogWarning("SHOWING FLOATING COMBAT TEXT");
        StartCoroutine(FloatingTextCoroutine(damageAmount, dmgType));
    }

    IEnumerator FloatingTextCoroutine(int damageAmount, DamageType dmgType)
    {
        float _popMovingTime = .2f;
        float _fadeTime = .2f;
        float yMovingAmount = 40f;
        float _showingTime = _popMovingTime + 1.5f;

        //Color _finalColor;

        if (dmgType == DamageType.HEALING)
            SetTextColor_Heal();
        else if (dmgType == DamageType.MANA)
            SetTextColor_Mana();
        else
            SetTextColor_Normal();

        floatingText.text = damageAmount.ToString();
        floatingText.color = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, 0);
        floatingText.DOColor(new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, 1), _fadeTime);
        floatingText.rectTransform.DOAnchorPosY(floatingText.rectTransform.anchoredPosition.y + yMovingAmount, _popMovingTime).OnComplete(BounceFloatingText);
        yield return new WaitForSeconds(_showingTime);
        floatingText.DOColor(new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, 0), _fadeTime);
        //yield return new WaitForSeconds(_fadeTime);
        //floatingText.rectTransform.localPosition = floatingTextPos;

        //new Color(0.18f, .63f, 1f); //light blue
    }

    void SetTextColor_Normal()
    {
        floatingText.color = Color.white;
    }

    void SetTextColor_Heal()
    {
        floatingText.color = healColor;
    }

    void SetTextColor_Mana()
    {
        floatingText.color = manaColor;
    }

    void BounceFloatingText()
    {
        floatingText.rectTransform.DOAnchorPosY(originalFloatingTextY, .4f).SetEase(Ease.OutBounce);
    }

    public void ShowDescriptionTooltip(string t)
    {
        descriptionTooltipText.transform.parent.parent.gameObject.SetActive(true);
        descriptionTooltipText.text = t;
    }

    public void HideDescriptionTooltip()
    {
        if (!descriptionTooltipText)
            return;

        descriptionTooltipText.transform.parent.parent.gameObject.SetActive(false);
    }

    //public void SetSkillNames()
    //{
    //    if (!techsPanel)
    //        return;

    //    for (int i = 0; i < characterBehaviour.Skills.Length; i++)
    //    {
    //        if (techsPanel.GetChild(0).GetChild(i).gameObject.activeSelf)
    //            techsPanel.GetChild(0).GetChild(i).GetComponent<TextMeshProUGUI>().text = characterBehaviour.Skills[i].actionName;
    //    }
    //}
}
