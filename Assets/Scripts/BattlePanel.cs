using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattlePanel : MonoBehaviour
{
    [SerializeField]EventSystem eventSystem;
    [SerializeField]GameObject darkOverlay;
    public EventSystem _EventSystem => eventSystem;

    [SerializeField] GameObject firstSelected;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        SetFirstSelected();
    }

    public void SetFirstSelected()
    {
        HideDarkOverlay();
        _EventSystem.SetSelectedGameObject(firstSelected);
    }

    public void ShowDarkOverlay()
    {
        darkOverlay.SetActive(true);
    }

    public void HideDarkOverlay()
    {
        darkOverlay.SetActive(false);
    }
}
