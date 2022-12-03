using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Item
{
    public InventoryItemData itemData;
    public int amount;
}

public class CharacterInventory : MonoBehaviour
{
    public List<Item> inventoryItens = new List<Item>();

}
