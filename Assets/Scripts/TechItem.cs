using UnityEngine;
using TMPro;

public class TechItem : MonoBehaviour
{
    int techIndex;
    CharacterBehaviour characterBehaviour;

    public void Initialize(int index, CombatActionSO combatActionSO)
    {
        techIndex = index;

        GetComponent<TextMeshProUGUI>().text = combatActionSO.actionName;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = combatActionSO.mpCost.ToString();
        characterBehaviour = GetComponentInParent<CharacterBehaviour>();
    }

    public void ShowDescription()
    {
        characterBehaviour.ShowDescription(techIndex);
    }

    public void SelectTech()
    {
        characterBehaviour.SelectTech(techIndex);
    }

}
