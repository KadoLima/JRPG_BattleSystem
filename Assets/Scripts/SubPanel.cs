using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SubPanel : MonoBehaviour
{
    [SerializeField] BattlePanel battlePanel;
    [SerializeField] Transform[] skillsItems;

    CharacterBehaviour player;

    private void Start()
    {
        player = GetComponentInParent<CharacterBehaviour>();

        for (int i = 0; i < player.Skills.Length; i++)
        {
            skillsItems[i].GetChild(0).GetComponent<TextMeshProUGUI>().text= player.Skills[i].mpCost.ToString();
        }

    }

    private void OnEnable()
    {
        if (player==null)
            player = GetComponentInParent<CharacterBehaviour>();

        for (int i = 0; i < player.Skills.Length; i++)
        {
            skillsItems[i].GetComponent<Button>().interactable = player.CurrentMP > player.Skills[i].mpCost;
        }
    }

    public void SetFirstSelected()
    {
        battlePanel.ShowDarkOverlay();
        battlePanel._EventSystem.SetSelectedGameObject(skillsItems[0].gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || 
            Input.GetKeyDown(KeyCode.A))
        {
            battlePanel.SetFirstSelected();
            this.gameObject.SetActive(false);
        }
    }
}
