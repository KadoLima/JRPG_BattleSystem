using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour
{
    [SerializeField] float scaleMultiplier = 1;

    Tweener tweener;
    Button button;
    RectTransform rectTransform;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
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

        rectTransform.DOAnchorPosX(rectTransform.anchoredPosition.x + 5, .1f).SetLoops(2, LoopType.Yoyo);
    }

}
