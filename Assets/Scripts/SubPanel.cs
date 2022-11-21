using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubPanel : MonoBehaviour
{
    [SerializeField] BattlePanel battlePanel;
    [SerializeField] GameObject firstSelected;
    [SerializeField] Transform skillsParent;

    CharacterBehaviour player;

    private void Start()
    {
        player = GetComponentInParent<CharacterBehaviour>();

        Debug.LogWarning(player.gameObject.name);
    }

    private void OnEnable()
    {
        if (player==null)
            player = GetComponentInParent<CharacterBehaviour>();

        for (int i = 0; i < player.Skills.Length; i++)
        {
            skillsParent.GetChild(i).GetComponent<Button>().interactable = player.CurrentMP > player.Skills[i].mpCost;
        }
    }

    public void SetFirstSelected()
    {
        battlePanel.ShowDarkOverlay();
        battlePanel._EventSystem.SetSelectedGameObject(firstSelected);
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
