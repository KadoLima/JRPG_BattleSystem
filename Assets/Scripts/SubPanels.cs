using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SubPanels : MonoBehaviour
{
    [SerializeField] BattlePanel battlePanel;
    [Header("TECHS SUBPANEL")]
    [SerializeField] List<Button> techItens = new List<Button>();
    [SerializeField] GameObject techItemPrefab;
    [SerializeField] GameObject techsSubPanel;
    [SerializeField] Transform techsContent;
    [Header("CONSUMABLES SUBPANEL")]
    [SerializeField] List<Button> itensList = new List<Button>();
    public List<Button> ItensList => itensList;
    [SerializeField] GameObject consumablePrefab;
    [SerializeField] GameObject itensSubpanel;
    [SerializeField] Transform itensContent;


    CharacterBehaviour player;

    private void OnEnable()
    {
        if (player == null)
            player = GetComponentInParent<CharacterBehaviour>();

        if (techItens.Count == 0)
            BuildTechItens();

        if (itensList.Count == 0)
            BuildComsumableItens();

        for (int i = 0; i < player.Skills.Length; i++)
        {
            techItens[i].GetComponent<Button>().interactable = player.CurrentMP > player.Skills[i].mpCost;
        }
    }

    private void Start()
    {
        player = GetComponentInParent<CharacterBehaviour>();

        techsSubPanel.SetActive(false);
        itensSubpanel.SetActive(false);
    }

    private void BuildTechItens()
    {
        for (int i = 0; i < player.Skills.Length; i++)
        {
            GameObject techItemPrefab = Instantiate(this.techItemPrefab, techsContent);
            techItemPrefab.GetComponent<TechItem>().Initialize(i, player.Skills[i]);
            //g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Skills[i].mpCost.ToString();
            techItens.Add(techItemPrefab.GetComponent<Button>());
        }

        SetNavigation(techItens);
    }

    private void BuildComsumableItens()
    {
        CharacterInventory playerInventory = player.GetComponent<CharacterInventory>();

        for (int i = 0; i < playerInventory.inventoryItens.Count; i++)
        {
            GameObject consumableItemPrefab = Instantiate(consumablePrefab, itensContent);
            consumableItemPrefab.GetComponent<ConsumableItem>().Initialize(playerInventory.inventoryItens[i],i);
            itensList.Add(consumableItemPrefab.GetComponent<Button>());
        }

        SetNavigation(itensList);
    }
    

    public void SetNavigation(List<Button> listItems)
    {
        for (int i = 0; i < listItems.Count; i++)
        {
            Navigation n = listItems[i].navigation;
            n.mode = Navigation.Mode.Explicit;

            if (i + 1 < listItems.Count && listItems[i + 1] != null)
            {
                n.selectOnDown = listItems[i + 1];  
            }

            if (i - 1 >= 0 && listItems[i-1] != null)
            {
                n.selectOnUp = listItems[i - 1];
            }

            listItems[i].navigation = n;

        }
    }

    public void SetFirstTechSelected()
    {
        battlePanel.ShowDarkOverlay();
        battlePanel._EventSystem.SetSelectedGameObject(techItens[0].gameObject);
    }

    public void SetFirstItemSelected()
    {
        battlePanel.ShowDarkOverlay();

        if (itensList.Count > 0)
            battlePanel._EventSystem.SetSelectedGameObject(itensList[0].gameObject);
    }

    public void HideSubPanels()
    {
        techsSubPanel.SetActive(false);
        itensSubpanel.SetActive(false);
    }
}
