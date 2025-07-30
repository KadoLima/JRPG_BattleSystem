using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public InventoryItemData itemData;
    public int amount;
}

public class CharacterInventory : MonoBehaviour
{
    [SerializeField] private List<Item> inventoryItens = new List<Item>();
    SubPanels characterSubPanels;

    public List<Item> InventoryItems => inventoryItens;

    private void Start()
    {
        characterSubPanels = transform.parent.GetComponentInChildren<CharacterUIController>().FindMainBattlePanel().SubPanels();
    }

    public void ConsumeItem(int itemIndex)
    {
        for (int i = 0; i < inventoryItens.Count; i++)
        {
            if (i == itemIndex)
            {
                inventoryItens[i].amount--;
                Button _itemUI = characterSubPanels.ItensList[i];
                
                if (inventoryItens[i].amount <= 0)
                {
                    Destroy(_itemUI.gameObject);
                    characterSubPanels.ItensList.Remove(_itemUI);
                    inventoryItens.Remove(inventoryItens[i]);

                    characterSubPanels.SetNavigation(characterSubPanels.ItensList);
                }
                else _itemUI.GetComponent<ConsumableItem>().UpdateAmountText(inventoryItens[i].amount);
            }
        }
    }
}
