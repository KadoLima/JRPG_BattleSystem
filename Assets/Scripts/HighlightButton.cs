using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour
{
    Tweener tweener;
    Button button;
    [SerializeField] float scaleMultiplier = 1;

    private void OnEnable()
    {
        button = GetComponentInChildren<Button>();

        if (button != null)
            button.interactable = true;
    }

    public void Selected()
    {
        tweener = transform.DOScale(1.075f * scaleMultiplier, .1f).SetUpdate(true);

        if (button)
            button.interactable = true;
    }

    public void Deselected()
    {
        tweener.Kill();
        transform.DOScale(1f, 0f).SetUpdate(true);
    }

    public void TweenRightYoyo()
    {
        if (button)
            button.interactable = false;

        RectTransform _rectTransform = GetComponent<RectTransform>();
        _rectTransform.DOAnchorPosX(_rectTransform.anchoredPosition.x + 5, .1f).SetLoops(2, LoopType.Yoyo);
    }

}
