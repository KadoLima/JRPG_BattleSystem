using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MainBattlePanel : MonoBehaviour
{
    EventSystem eventSystem;
    public EventSystem MyEventSystem => eventSystem;

    [SerializeField]GameObject darkOverlay;
    [SerializeField] GameObject swapCharacterIndicator;

    [SerializeField] GameObject firstSelected;

    void Awake()
    {
        eventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, .2f).SetEase(Ease.OutBack);

        SetFirstSelected();
        HideSubPanels();
    }

    public void SetFirstSelected()
    {
        HideDarkOverlay();
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void ShowDarkOverlay()
    {
        darkOverlay.SetActive(true);
    }

    public void HideDarkOverlay()
    {
        darkOverlay.SetActive(false);
    }

    public void HideSubPanels()
    {
        SubPanels _subPanels = GetComponent<SubPanels>();
        _subPanels.HideSubPanels();
    }

    public SubPanels SubPanels()
    {
        return GetComponent<SubPanels>();
    }

    public void ShowHideSwapCharsIndicator(bool state)
    {
        swapCharacterIndicator.SetActive(state);
    }

}
