using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    //[field: SerializeField] public Transform GetAttackedPos { get ; private set; }
    //[SerializeField] CombatEffects combatEffects;
    [SerializeField] float chanceToUseSkill;
    public float ChanceToUseSkill => chanceToUseSkill;
    CharacterBehaviour currentPlayerTarget;
    public CharacterBehaviour CurrentPlayerTarget => currentPlayerTarget;

    // Start is called before the first frame update
    public override void Start()
    {
        UIController = GetComponent<CharacterUIController>();
        CombatManager.instance.enemiesOnField.Add(this);
        originalPosition = this.transform.localPosition;
        currentHP = myStats.baseHP;
    }

    //public override void Update()
    //{

    //}

    public CharacterBehaviour GetRandomPlayer()
    {
        currentPlayerTarget = CombatManager.instance.playersOnField[Random.Range(0, CombatManager.instance.playersOnField.Count)];
        return currentPlayerTarget;
    }

    public void ShowPointer()
    {
        UIController.ShowHidePointer(true);
    }

    public void HidePointer()
    {
        UIController.ShowHidePointer(false);
    }
}
