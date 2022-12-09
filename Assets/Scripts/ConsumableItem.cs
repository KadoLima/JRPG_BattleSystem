using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableItem : MonoBehaviour
{
    [SerializeField]Image image;
    [SerializeField] TextMeshProUGUI amountText;
    InventoryItemData inventoryItemData;

    //int consumableIndex;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(Item item, int index)
    {
        //consumableIndex = index;
        inventoryItemData = item.itemData;
        GetComponent<TextMeshProUGUI>().text = item.itemData.name;
        image.sprite = item.itemData.itemSprite;
        amountText.text = item.amount.ToString();
    }

    public void ShowDescription()
    {
        CharacterBehaviour player = GetComponentInParent<CharacterBehaviour>();
        player.ShowDescription(inventoryItemData.itemDescription);
    }

    public void UseItem()
    {
        CharacterBehaviour player = GetComponentInParent<CharacterBehaviour>();
        player.SelectConsumableItem(transform.GetSiblingIndex(), inventoryItemData.damageType);
    }

    public void UpdateAmountText(int amount)
    {
        Debug.LogWarning("Updating amount...");
        amountText.text = amount.ToString();
    }
}
