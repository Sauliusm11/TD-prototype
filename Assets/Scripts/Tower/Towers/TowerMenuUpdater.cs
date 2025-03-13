using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerMenuUpdater : MonoBehaviour
{
    [SerializeField]
    TMP_Text sellPriceText;
    [SerializeField]
    TMP_Text TowerNameText;
    
    public void UpdateTowerMenu(string name, int price)
    {
        sellPriceText.text = "Sell for: " + price.ToString();
        //Full tower object name is "Tower (Clone)", want to show only "Tower"
        TowerNameText.text = name.Remove(name.Length-7,7);
    }
}
