using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public InventoryItemData itemData;
    public int amount;
}

public class CharacterInventory : MonoBehaviour
{
    public List<Item> inventoryItens = new List<Item>();
    SubPanels characterSubPanels;

    private void Start()
    {
        characterSubPanels = transform.parent.GetComponentInChildren<CharacterUIController>().GetBattlePanel().SubPanels();
    }

    public void ConsumeItem(int itemIndex)
    {
        Debug.LogWarning("Consuming an item...");

        for (int i = 0; i < inventoryItens.Count; i++)
        {
            if (i == itemIndex)
            {
                //Debug.LogWarning("CONSUMING " + inventoryItens[i].itemData.itemName);
                inventoryItens[i].amount--;
                var _itemUI = characterSubPanels.ItensList[i];
                
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
