using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UpgradeHandler : MonoBehaviour
{
    Money moneyHandler;
    TowerSelectionHandler towerSelectionHandler;
    ShootingHandler shootingHandler;
    ObjectPooling currentPooler;
    TowerContainer towerContainer;
    Tilemap tilemap;
    PathfindingManager pathfindingManager;


    int moneySpent;
    float coolDown;
    float range;
    int damage;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
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
        Vector3Int cellPosition = tilemap.WorldToCell(gameObject.transform.position);
        pathfindingManager.RemoveTowerFromNode(cellPosition);
        currentPooler.DeactivateObject(gameObject);
    }
    public int GetSellCost()
    {
        return moneySpent / 2;
    }
}
