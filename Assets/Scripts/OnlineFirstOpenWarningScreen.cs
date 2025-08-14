using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlineFirstOpenWarningScreen : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button firstSelected;

    private void Start()
    {
        firstSelected.onClick.AddListener(Hide);

        Invoke(nameof(Show), 2f);
    }

    private void Show()
    {
        panel.localScale = Vector3.zero;
        content.gameObject.SetActive(true);
        panel.DOScale(1, 0.25f).SetEase(Ease.OutBack);

        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);

        firstSelected.GetComponent<RectTransform>().DOScale(Vector3.one * 1.05f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void Hide()
    {
        content.gameObject.SetActive(false);
    }

}
