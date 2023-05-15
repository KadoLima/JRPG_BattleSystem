using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TechItem : MonoBehaviour
{
    int techIndex;

    public void Initialize(int index, CombatActionSO combatActionSO)
    {
        techIndex = index;

        GetComponent<TextMeshProUGUI>().text = combatActionSO.actionName;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = combatActionSO.mpCost.ToString();
    }

    public void ShowDescription()
    {
        CharacterBehaviour _player = GetComponentInParent<CharacterBehaviour>();
        _player.ShowDescription(techIndex);
    }

    public void SelectTech()
    {
        CharacterBehaviour _player = GetComponentInParent<CharacterBehaviour>();
        _player.SelectTech(techIndex);
    }

}
