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

    CharacterBehaviour characterBehaviour;

    public void ExecuteAction()
    {
        if (panelToOpen.panel)
        {
            if (characterBehaviour == null)
                characterBehaviour = GetComponentInParent<CharacterBehaviour>();

            characterBehaviour.ChangeBattleState(panelToOpen.battleState);
            panelToOpen.panel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(GetFirstActiveChild());
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
