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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(Item item)
    {
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

    }
}
