using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerXPItem : MonoBehaviour
{

    [System.Serializable]
    public class XPInfo
    {
        public float currentXP = 3755;
        public float xpToLevel = 5000;
        public int currentLevel = 3;
    }

    [Header("XP Info example for this prototype")]
    [SerializeField] XPInfo playerXPInfo;
    [Space(20)]
    [SerializeField] Image xpBarFill;
    [SerializeField] GameObject levelUpWarning;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] TextMeshProUGUI levelText;

    // Start is called before the first frame update
    void Start()
    {
        levelUpWarning.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateXPText();
        UpdateLevelText();
        Debug.LogWarning(playerXPInfo.currentXP / playerXPInfo.xpToLevel);
        xpBarFill.fillAmount = playerXPInfo.currentXP / playerXPInfo.xpToLevel;

        StartCoroutine(EarnXPCoroutine());
    }

    private void UpdateXPText()
    {
        xpText.text = playerXPInfo.currentXP + " / " + playerXPInfo.xpToLevel;
    }

    private void UpdateLevelText()
    {
        levelText.text = "Lvl. " + playerXPInfo.currentLevel;
    }

    IEnumerator EarnXPCoroutine()
    {
        int remainingValueToLevel = (int)(playerXPInfo.xpToLevel - playerXPInfo.currentXP);
        int overflowXP = CombatManager.instance.TotalXPEarned() - remainingValueToLevel;

        if (overflowXP <= 0) //NO OVERFLOW XP
        {
            int resultXP = (int)(playerXPInfo.currentXP + CombatManager.instance.TotalXPEarned());
            while (playerXPInfo.currentXP < resultXP)
            {
                AddToXP(resultXP);
                yield return null;
            }
        }
        else
        {
            while (playerXPInfo.currentXP < playerXPInfo.xpToLevel)
            {
                AddToXP((int)playerXPInfo.xpToLevel);
                yield return null;
            }

            playerXPInfo.currentLevel++;
            playerXPInfo.xpToLevel = Mathf.RoundToInt(playerXPInfo.xpToLevel * 1.06f);
            UpdateLevelText();
            levelUpWarning.SetActive(true);
            xpBarFill.fillAmount = 0;
            playerXPInfo.currentXP = 0;
            UpdateXPText();
            
            while (playerXPInfo.currentXP < overflowXP)
            {
                AddToXP(overflowXP);
                yield return null;
            }
        }

    }

    private void AddToXP(int maxValue)
    {
        playerXPInfo.currentXP+=Random.Range(9,20);
        playerXPInfo.currentXP = Mathf.Clamp(playerXPInfo.currentXP, 0, maxValue);
        UpdateXPText();
        xpBarFill.fillAmount = playerXPInfo.currentXP / playerXPInfo.xpToLevel;
    }
}
