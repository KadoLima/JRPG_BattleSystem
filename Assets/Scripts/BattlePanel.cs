using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BattlePanel : MonoBehaviour
{
    [SerializeField]EventSystem eventSystem;
    [SerializeField]GameObject darkOverlay;
    [SerializeField] GameObject swapCharacterIndicator;
    public EventSystem MyEventSystem => eventSystem;

    [SerializeField] GameObject firstSelected;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, .2f).SetEase(Ease.OutBack);

        SetFirstSelected();
    }

    public void SetFirstSelected()
    {
        HideDarkOverlay();
        MyEventSystem.SetSelectedGameObject(firstSelected);
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
        SubPanels subPanels = GetComponent<SubPanels>();
        subPanels.HideSubPanels();
    }

    public SubPanels GetSubPanels()
    {
        return GetComponent<SubPanels>();
    }

    public void ShowHideSwapCharsIndicator(bool state)
    {
        swapCharacterIndicator.SetActive(state);
    }

}
