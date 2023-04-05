using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character Stats/New Character Stats")]
public class CharacterStats : ScriptableObject
{
    public Sprite characterSprite;
    public int baseHP;
    public int baseMP;
    public int minDamage;
    public int maxDamage;
    public float rechargeRate;
    public float critChance;

    public int baseDamage() => Random.Range(minDamage, maxDamage);
}
