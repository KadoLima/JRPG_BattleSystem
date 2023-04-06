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


    public void Initialize(Item item, int index)
    {
        inventoryItemData = item.itemData;
        GetComponent<TextMeshProUGUI>().text = item.itemData.name;
        image.sprite = item.itemData.itemSprite;
        amountText.text = item.amount.ToString();
    }

    public void ShowDescription()
    {
        CharacterUIController _playerUI = GetComponentInParent<CharacterBehaviour>().UIController;
        _playerUI.ShowDescriptionTooltip(inventoryItemData.itemDescription);
    }

    public void UseItem()
    {
        CharacterBehaviour _player = GetComponentInParent<CharacterBehaviour>();
        _player.SelectConsumableItem(transform.GetSiblingIndex(), inventoryItemData.damageType);
    }

    public void UpdateAmountText(int amount)
    {
        amountText.text = amount.ToString();
    }
}
