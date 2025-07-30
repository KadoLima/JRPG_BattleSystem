using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubPanels : MonoBehaviour
{
    MainBattlePanel mainBattlePanel;
    [Header("TECHS SUBPANEL")]
    [SerializeField] List<Button> techItens = new List<Button>();
    [SerializeField] GameObject techItemPrefab;
    [SerializeField] GameObject techsSubPanel;
    [SerializeField] Transform techsContent;
    [Header("CONSUMABLES SUBPANEL")]
    [SerializeField] List<Button> itensList = new List<Button>();
    [SerializeField] GameObject consumablePrefab;
    [SerializeField] GameObject itensSubpanel;
    [SerializeField] Transform itensContent;

    CharacterBehaviour player;
    CharacterInventory playerInventory;
    public List<Button> ItensList => itensList;

    void Awake()
    {
        mainBattlePanel = GetComponent<MainBattlePanel>();
        player = GetComponentInParent<CharacterBehaviour>();
        playerInventory = player.GetComponentInChildren<CharacterInventory>();
    }

    private void OnEnable()
    {
        if (techItens.Count == 0)
            BuildTechItens();

        if (itensList.Count == 0)
        {
            BuildComsumableItens();
        }
    }

    public void CheckSkillsAvailability()
    {
        for (int i = 0; i < SkillsCount(); i++)
        {
            techItens[i].GetComponent<Button>().interactable = player.CurrentMP >= player.CharacterActions[2 + i].mpCost;
        }
    }

    private void Start()
    {
        techsSubPanel.SetActive(false);
        itensSubpanel.SetActive(false);
    }

    private void BuildTechItens()
    {
        for (int i = 0; i < SkillsCount(); i++)
        {
            GameObject _techItemPrefab = Instantiate(this.techItemPrefab, techsContent);
            _techItemPrefab.GetComponent<TechItem>().Initialize(i, player.CharacterActions[2 + i]);
            techItens.Add(_techItemPrefab.GetComponent<Button>());
        }

        SetNavigation(techItens);
    }

    private void BuildComsumableItens()
    {
        for (int i = 0; i < playerInventory.InventoryItems.Count; i++)
        {
            GameObject consumableItemPrefab = Instantiate(consumablePrefab, itensContent);
            consumableItemPrefab.GetComponent<ConsumableItem>().Initialize(playerInventory.InventoryItems[i],i);
            itensList.Add(consumableItemPrefab.GetComponent<Button>());
        }

        SetNavigation(itensList);
    }
    

    public void SetNavigation(List<Button> listItems)
    {
        for (int i = 0; i < listItems.Count; i++)
        {
            Navigation _nav = listItems[i].navigation;
            _nav.mode = Navigation.Mode.Explicit;

            if (i + 1 < listItems.Count && listItems[i + 1] != null)
            {
                _nav.selectOnDown = listItems[i + 1];  
            }

            if (i - 1 >= 0 && listItems[i-1] != null)
            {
                _nav.selectOnUp = listItems[i - 1];
            }

            listItems[i].navigation = _nav;
        }
    }

    public void SetFirstTechSelected()
    {
        mainBattlePanel.ShowDarkOverlay();
        mainBattlePanel.MyEventSystem.SetSelectedGameObject(techItens[0].gameObject);
    }

    public void SetFirstItemSelected()
    {
        mainBattlePanel.ShowDarkOverlay();

        if (itensList.Count > 0)
            mainBattlePanel.MyEventSystem.SetSelectedGameObject(itensList[0].gameObject);
    }

    public void HideSubPanels()
    {
        techsSubPanel.SetActive(false);
        itensSubpanel.SetActive(false);
    }

    private int SkillsCount()
    {
        int _skillsCount = 0;

        foreach (var a in player.CharacterActions)
        {
            if (a.actionType == ActionType.SKILL)
                _skillsCount++;
        }

        return _skillsCount;
    }
}
