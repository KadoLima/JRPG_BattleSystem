using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleOption : MonoBehaviour
{
    [System.Serializable]
    public struct PanelToOpen
    {
        public GameObject panel;
        public BattleState battleState;
    }

    [SerializeField] PanelToOpen panelToOpen;

    public void ExecuteAction()
    {
        if (panelToOpen.panel)
        {
            GetComponentInParent<CharacterBehaviour>().ChangeBattleState(panelToOpen.battleState);
            panelToOpen.panel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(GetFirstActiveChild());
            return;
        }
    }

    private GameObject GetFirstActiveChild()
    {
        GameObject _firstSelected = null;

        for (int i = 0; i < panelToOpen.panel.transform.GetChild(0).childCount; i++)
        {
            if (panelToOpen.panel.transform.GetChild(0).GetChild(i).gameObject.activeSelf)
            {
                _firstSelected = panelToOpen.panel.transform.GetChild(0).GetChild(i).gameObject;
                break;
            }
        }

        return _firstSelected;
    }
}
