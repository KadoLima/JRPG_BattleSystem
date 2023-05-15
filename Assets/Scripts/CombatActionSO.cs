using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    NORMAL_ATTACK,
    SKILL,
    ITEM,
    RECHARGING
}

public enum DamageType
{
    HARMFUL,
    HEALING,
    MANA,
    UNDEFINED
}


[CreateAssetMenu(fileName = "New CombatAction", menuName = "Combat Actions/New CombatAction")]
[System.Serializable]
public class CombatActionSO : ScriptableObject
{
    public ActionType actionType;
    public DamageType damageType;
    public string actionName;
    public string description;
    public int mpCost;
    public GameObject projectile;
    public bool goToTarget;
    public bool isAreaOfEffect;
    public float damageMultiplier;
    public AnimationCycle animationCycle;

    public bool IsHarmful => this.damageType == DamageType.HARMFUL;

}
