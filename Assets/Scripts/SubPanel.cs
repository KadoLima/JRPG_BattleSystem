using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubPanel : MonoBehaviour
{
    [SerializeField] BattlePanel battlePanel;
    [SerializeField] GameObject firstSelected;

    private void Start()
    {
       
    }

    public void SetFirstSelected()
    {
        battlePanel.ShowDarkOverlay();
        battlePanel._EventSystem.SetSelectedGameObject(firstSelected);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.A))
        {
            battlePanel.SetFirstSelected();
            this.gameObject.SetActive(false);
        }
    }
}
