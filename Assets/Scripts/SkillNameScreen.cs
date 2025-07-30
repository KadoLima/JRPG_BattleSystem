using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SkillNameScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] float startPosY = 28;
    [SerializeField] float endPosY = -24;

    RectTransform rectTransform;

    private void OnEnable()
    {
        CharacterBehaviour.OnSkillUsed += Show;
        EnemyBehaviour.OnEnemyUsedSkill += Show;
    }

    private void OnDisable()
    {
        CharacterBehaviour.OnSkillUsed -= Show;
        EnemyBehaviour.OnEnemyUsedSkill -= Show;
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPos(new Vector2(this.rectTransform.anchoredPosition.x,startPosY), 0);
    }

    public void Show(string textToShow)
    {
        if (string.IsNullOrEmpty(textToShow))
            return;

        StartCoroutine(Show_Coroutine(textToShow));
    }

    IEnumerator Show_Coroutine(string textToShow)
    {
        skillNameText.text = textToShow;
        rectTransform.DOAnchorPos(new Vector2(this.rectTransform.anchoredPosition.x, endPosY), 0.15f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1.8f);
        rectTransform.DOAnchorPos(new Vector2(this.rectTransform.anchoredPosition.x, startPosY), 0.15f).SetEase(Ease.Linear);
    }
}
