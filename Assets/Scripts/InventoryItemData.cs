using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "InventoryItem/New Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public enum ItemType
    {
        CONSUMABLE,
        EQUIPMENT,
        NULL
    }

    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
    public ItemType itemType;
    public DamageType damageType;
    public int effectAmount;
    public int effectTurns;

public void ApplyItemEffect()
{
        CharacterBehaviour currentPlayer = CombatManager.instance.CurrentActivePlayer;

    switch (damageType)
        {
            case DamageType.HEALING:
                currentPlayer.IncreaseHP(effectAmount);
                break;
            case DamageType.MANA:
                currentPlayer.IncreaseMP(effectAmount);
                break;
        }
}
}
