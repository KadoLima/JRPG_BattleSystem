using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsumableItem : MonoBehaviour
{
    [SerializeField]Image image;
    [SerializeField] TextMeshProUGUI amountText;
    InventoryItemData inventoryItemData;

    CharacterBehaviour player;
    CharacterUIController playerUI;

    public void Initialize(Item item, int index)
    {
        inventoryItemData = item.itemData;
        GetComponent<TextMeshProUGUI>().text = item.itemData.name;
        image.sprite = item.itemData.itemSprite;
        amountText.text = item.amount.ToString();

        player = GetComponentInParent<CharacterBehaviour>();
        playerUI = player.CharacterUIController;
    }

    public void ShowDescription()
    {
        if (playerUI)
            playerUI.ShowDescriptionTooltip(inventoryItemData.itemDescription);
        else Debug.LogWarning("No playerUI. // " + this.gameObject.name);
    }

    public void UseItem()
    {
        player.SelectConsumableItem(transform.GetSiblingIndex(), inventoryItemData.damageType);
    }

    public void UpdateAmountText(int amount)
    {
        amountText.text = amount.ToString();
    }
}
