using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class SkillNameScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] float startPosY = 28;
    [SerializeField] float endPosY = -24;

    RectTransform rectTransform;

    public static SkillNameScreen instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPos(new Vector2(this.rectTransform.anchoredPosition.x,startPosY), 0);
    }


    public void Show(string textToShow)
    {
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
