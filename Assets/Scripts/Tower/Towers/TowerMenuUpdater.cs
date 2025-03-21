using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    TowerContainer.Upgrade nextUpgrade;

    UpgradeHandler currentTower;

    private void FixedUpdate()
    {
        if (currentTower != null)
        {
            //This should become function to gray it out
            upgrade1Button.SetActive(currentTower.IsUpgradeAvailable());
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
            upgrade1ButtonText.text = "Tier " + nextUpgrade.tier + " upgrade for: " + nextUpgrade.cost;
            tierText.text = "Tier: " + (nextUpgrade.tier-1).ToString();
        }
        else 
        {
            tierText.text = "Tier: max";
        }
        attackDamageText.text = "Attack damage:"+ tower.GetAttackDamage().ToString();
        attackSpeedText.text = "Fire rate:" + tower.GetAttackSpeed().ToString()+"/s";
        attackRangeText.text = "Attack range:"+ tower.GetAttackRange().ToString();

    }
}
