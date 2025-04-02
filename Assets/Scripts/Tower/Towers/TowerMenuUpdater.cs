using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerMenuUpdater : MonoBehaviour
{
    [SerializeField]
    TMP_Text sellPriceText;
    [SerializeField]
    TMP_Text attackDamageText;
    [SerializeField]
    TMP_Text attackSpeedText;
    [SerializeField]
    TMP_Text attackRangeText;
    [SerializeField]
    TMP_Text towerNameText;
    [SerializeField]
    TMP_Text tierText;
    [SerializeField]
    TMP_Text upgrade1ButtonText;
    [SerializeField]
    TMP_Text upgrade2ButtonText;
    [SerializeField]
    GameObject upgrade1Button;
    [SerializeField]
    GameObject upgrade2Button;
    Button upgrade1SpriteRenderer;
    Button upgrade2SpriteRenderer;
    TowerContainer.Upgrade nextUpgrade;
    UpgradeHandler currentTower;

    private void Start()
    {
        upgrade1SpriteRenderer = upgrade1Button.GetComponent<Button>();
        upgrade2SpriteRenderer = upgrade2Button.GetComponent<Button>();
    }
    private void FixedUpdate()
    {
        if (currentTower != null)
        {
            SetButtonState(1,currentTower.IsUpgradeAvailable());
        }
    }
    private void SetButtonState(int buttonNumber, bool state)
    {
        Color newColor = Color.white;
        if (!state)
        {
            newColor = new Color(1, 1, 1, 0.5f);
        }
        switch (buttonNumber)
        {
            case 1:
                upgrade1SpriteRenderer.interactable = state;
                break;
            case 2:
                upgrade2SpriteRenderer.interactable = state;
                break;
            default:
                break;
        }
    }
    public void UpdateTowerMenu(UpgradeHandler tower)
    {
        currentTower = tower;
        //Full tower object name is "Tower (Clone)", want to show only "Tower"
        towerNameText.text = tower.name.Remove(tower.name.Length - 7, 7);
        sellPriceText.text = "Sell for: " + tower.GetSellCost().ToString();
        nextUpgrade = tower.GetUpgrade();
        if (nextUpgrade.tier != -1)
        {
            upgrade1Button.SetActive(true);
            upgrade1ButtonText.text = "Tier " + nextUpgrade.tier + " upgrade for: " + nextUpgrade.cost;
            if(nextUpgrade.tier == 3)
            {
                upgrade2Button.SetActive(true);
                upgrade1ButtonText.text = "Elite upgrade for: " + nextUpgrade.cost;
                upgrade2ButtonText.text = "Alternate elite upgrade for: " + tower.GetSecondaryElite().cost; 
            }
            tierText.text = "Tier: " + (nextUpgrade.tier-1).ToString();
        }
        else 
        {
            tierText.text = "Tier: max";
            upgrade1Button.SetActive(false);
            upgrade2Button.SetActive(false);
        }
        attackDamageText.text = "Attack damage:"+ tower.GetAttackDamage().ToString();
        attackSpeedText.text = "Fire rate:" + tower.GetAttackSpeed().ToString()+"/s";
        attackRangeText.text = "Attack range:"+ tower.GetAttackRange().ToString();

    }
}
