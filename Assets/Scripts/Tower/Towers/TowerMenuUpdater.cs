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


    public void UpdateTowerMenu(UpgradeHandler tower)
    {
        sellPriceText.text = "Sell for: " + tower.GetSellCost().ToString();
        attackDamageText.text = "Attack damage:"+ tower.GetAttackDamage().ToString();
        attackSpeedText.text = "Fire rate:" + tower.GetAttackSpeed().ToString()+"/s";
        attackRangeText.text = "Attack range:"+ tower.GetAttackRange().ToString();
        //Full tower object name is "Tower (Clone)", want to show only "Tower"
        towerNameText.text = tower.name.Remove(tower.name.Length-7,7);
    }
}
