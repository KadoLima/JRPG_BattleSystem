using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CharacterUIController : MonoBehaviour
{
    [SerializeField] CanvasGroup myCanvasGroup;
    [SerializeField] GameObject pointer;
    [SerializeField] GameObject playerIndicator;
    [SerializeField] MainBattlePanel battlePanel;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;
    [Header("Floating Combat Text")]
    [SerializeField] TextMeshProUGUI floatingText;
    GameObject criticalText;
    [SerializeField] Color healColor;
    [SerializeField] Color manaColor;
    [Space(10)]
    [SerializeField] Transform descriptionTooltipContainer;
    [SerializeField] TextMeshProUGUI descriptionTooltipText;
    [SerializeField] GameObject chatBubble;
    [field: SerializeField] public Image cooldownBar { get; private set; }

    CharacterBehaviour characterBehaviour;
    float originalFloatingTextY;
    CanvasGroup cooldownBarCanvasGroup;
    CanvasGroup floatingTextCanvasGroup;

    void Start()
    {
        if (chatBubble!=null)
            chatBubble.SetActive(false);

        pointer.SetActive(false);
        criticalText = floatingText.transform.GetChild(0).gameObject;
        criticalText.SetActive(false);

        if (cooldownBar)
        {
            cooldownBar.fillAmount = 0;
            cooldownBarCanvasGroup = cooldownBar.GetComponentInParent<CanvasGroup>();
        }

        characterBehaviour = GetComponentInParent<CharacterBehaviour>();

        if (battlePanel)
            battlePanel.gameObject.SetActive(false);
        
        if (floatingText)
            floatingTextCanvasGroup = floatingText.GetComponent<CanvasGroup>();

        Invoke(nameof(RefreshHPMP), .1f);

        ResetFloatingText();

        if (playerIndicator)
            playerIndicator.SetActive(GameManager.IsOnline() && characterBehaviour.IsOwner);
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

        HideMainBattlePanel();

        myCanvasGroup.alpha = 0;
    }

    public MainBattlePanel FindMainBattlePanel()
    {
        if (!battlePanel)
            return null;

        return battlePanel;
    }

     public void ShowMainBattlePanel()
    {
        if (!battlePanel)
            return;

        battlePanel.gameObject.SetActive(true);
        battlePanel.SetFirstSelected();
        battlePanel.HideSubPanels();
        HideDescriptionTooltip();
        HidePointer();
    }

    public void HideMainBattlePanel()
    {

        if (!battlePanel)
            return;

        battlePanel.HideSubPanels();
        battlePanel.gameObject.SetActive(false);
    }

    public void ShowPointer() => pointer.SetActive(true);
    public void HidePointer() => pointer.SetActive(false);

    public void HideUI()
    {
        hpText.DOColor(new Color(0, 0, 0, 0f), .2f);

        if (mpText)
            mpText.DOColor(new Color(0, 0, 0, 0f), .2f);

        if (cooldownBar)
            cooldownBarCanvasGroup.alpha = 0;
    }

    public void ShowUI()
    {
        hpText.DOColor(new Color(0, 0, 0, 1f), .2f);

        if (mpText)
            mpText.DOColor(new Color(0, 0, 0, 1f), .2f);

        if (cooldownBar)
            cooldownBarCanvasGroup.alpha = 1;
    }

    public void RefreshHP(int currentHP, int baseHP)
    {
        hpText.text = $"H: {Mathf.Max(0, currentHP)}/{baseHP}";
    }

    public void RefreshMP(int currentMP, int baseMP)
    {
        if (mpText == null)
            return;

        mpText.text = $"M: {Mathf.Max(0, currentMP)}/{baseMP}";
    }

    public void ShowFloatingDamageText(int damageAmount, DamageType dmgType, bool isCrit)
    {
        StartCoroutine(FloatingTextCoroutine(damageAmount, dmgType, isCrit));
    }

    IEnumerator FloatingTextCoroutine(int damageAmount, DamageType dmgType, bool isCrit)
    {
        float _popMovingTime = .2f;
        float _fadeTime = .2f;
        float _yMovingAmount = 40f;
        float _showingTime = _popMovingTime + 1f;

        if (dmgType == DamageType.HEALING)
            SetTextColor_Heal();
        else if (dmgType == DamageType.MANA)
            SetTextColor_Mana();
        else
            SetTextColor_Normal();

        floatingText.text = damageAmount.ToString();
        floatingTextCanvasGroup.DOFade(0, 0);
        floatingTextCanvasGroup.DOFade(1, _fadeTime);
        criticalText.SetActive(isCrit);
        floatingText.rectTransform.DOAnchorPosY(floatingText.rectTransform.anchoredPosition.y + _yMovingAmount, _popMovingTime).OnComplete(BounceFloatingText);
        yield return new WaitForSeconds(_showingTime);
        floatingTextCanvasGroup.DOFade(0, _fadeTime);
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
        descriptionTooltipContainer.gameObject.SetActive(true);
        descriptionTooltipText.text = t;
    }

    public void HideDescriptionTooltip()
    {
        if (!descriptionTooltipText)
            return;

        descriptionTooltipContainer.gameObject.SetActive(false);
    }

    public void ShowChatBubble()
    {
        chatBubble.SetActive(true);
    }

    public void HideChatBubble()
    {
        chatBubble.SetActive(false);
    }
}
