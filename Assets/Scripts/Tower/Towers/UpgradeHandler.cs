using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UpgradeHandler : MonoBehaviour
{
    Money moneyHandler;
    TowerSelectionHandler towerSelectionHandler;
    ShootingHandler shootingHandler;
    ObjectPooling currentPooler;
    TowerContainer towerContainer;


    int moneySpent;
    float coolDown;
    float range;
    int damage;
    // Start is called before the first frame update
    void Start()
    {
        towerContainer = TowerContainer.getInstance();
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        shootingHandler = gameObject.GetComponent<ShootingHandler>();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        if (towerManagerObject != null)
        {
            towerSelectionHandler = towerManagerObject.GetComponent<TowerSelectionHandler>();
        }
        TowerContainer.Tower selection;
        foreach (TowerContainer.Tower tower in towerContainer.towers)
        {
            if (gameObject.name.Contains(tower.name))
            {
                selection = tower;
                currentPooler = towerSelectionHandler.GetTowerPoolerFromSelection(selection);
                coolDown = tower.attackSpeed;
                range = tower.attackRange;
                damage = tower.attackDamage;
                moneySpent = tower.cost;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SellTower()
    {
        moneyHandler.AddMoney(GetSellCost());
        currentPooler.DeactivateObject(gameObject);
    }
    public int GetSellCost()
    {
        return moneySpent / 2;
    }
}
