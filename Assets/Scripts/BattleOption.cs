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
    EventSystem currentEventSystem;

    private void Start()
    {
        characterBehaviour = GetComponentInParent<CharacterBehaviour>();
        currentEventSystem = EventSystem.current;
    }

    public void ExecuteAction()
    {
        if (panelToOpen.panel)
        {
            characterBehaviour.ChangeBattleState(panelToOpen.battleState);
            panelToOpen.panel.SetActive(true);
            currentEventSystem.SetSelectedGameObject(GetFirstActiveChild());
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
