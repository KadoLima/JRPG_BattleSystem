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
        characterSubPanels = GetComponent<CharacterUIController>().GetBattlePanel().GetSubPanels();
    }

    public void ConsumeItem(int itemIndex)
    {
        for (int i = 0; i < inventoryItens.Count; i++)
        {
            if (i == itemIndex)
            {
                Debug.LogWarning("CONSUMING " + inventoryItens[i].itemData.itemName);
                inventoryItens[i].amount--;
                var itemUI = characterSubPanels.ItensList[i];
                
                if (inventoryItens[i].amount <= 0)
                {
                    Destroy(itemUI.gameObject);
                    characterSubPanels.ItensList.Remove(itemUI);
                    inventoryItens.Remove(inventoryItens[i]);

                    characterSubPanels.SetNavigation(characterSubPanels.ItensList);
                }
                else itemUI.GetComponent<ConsumableItem>().UpdateAmountText(inventoryItens[i].amount);
            }
        }
    }
}
