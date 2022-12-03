using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SubPanels : MonoBehaviour
{
    [SerializeField] BattlePanel battlePanel;
    [Header("TECHS SUBPANEL")]
    [SerializeField] List<Button> techItens = new List<Button>();
    [SerializeField] GameObject techItemPrefab;
    [SerializeField] GameObject techsSubPanel;
    [SerializeField] Transform techsContent;
    [Header("CONSUMABLES SUBPANEL")]
    [SerializeField] List<Button> consumableItens = new List<Button>();
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

        if (consumableItens.Count == 0)
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

    void Update()
    {
        BackIfKeyPressed();
    }

    private void BackIfKeyPressed()
    {
        if (player.CurrentBattlePhase == BattleState.SELECTING_TECH ||
            player.CurrentBattlePhase == BattleState.SELECTING_ITEM)
        {
            if (Input.GetKeyDown(KeyCode.Backspace) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.A))
            {
                techsSubPanel.SetActive(false);
                itensSubpanel.SetActive(false);
                player.ChangeBattleState(BattleState.READY);
            }
        }
    }

    private void BuildTechItens()
    {
        for (int i = 0; i < player.Skills.Length; i++)
        {
            GameObject g = Instantiate(techItemPrefab, techsContent);
            g.GetComponent<TechItem>().Initialize(i, player.Skills[i]);
            //g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = player.Skills[i].mpCost.ToString();
            techItens.Add(g.GetComponent<Button>());
        }

        SetNavigation(techItens);
    }

    private void BuildComsumableItens()
    {
        CharacterInventory playerInventory = player.GetComponent<CharacterInventory>();

        for (int i = 0; i < playerInventory.inventoryItens.Count; i++)
        {
            GameObject g = Instantiate(consumablePrefab, itensContent);
            g.GetComponent<ConsumableItem>().Initialize(playerInventory.inventoryItens[i]);
            consumableItens.Add(g.GetComponent<Button>());
        }

        SetNavigation(consumableItens);
    }

    void SetNavigation(List<Button> listItems)
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

    public void SetFirstSelected()
    {
        battlePanel.ShowDarkOverlay();
        battlePanel._EventSystem.SetSelectedGameObject(techItens[0].gameObject);
    }

    public void HideSubPanels()
    {
        techsSubPanel.SetActive(false);
        itensSubpanel.SetActive(false);
    }


}
