using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOption : MonoBehaviour
{
    [SerializeField] GameObject panelToOpen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ExecuteAction()
    {
        if (panelToOpen)
        {
            panelToOpen.SetActive(true);
            panelToOpen.GetComponent<SubPanel>().SetFirstSelected();
            return;
        }
    }
}
