using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TowerContainer;

public class UpgradeHandler : MonoBehaviour
{
    Money moneyHandler;
    TowerSelectionHandler towerSelectionHandler;
    ShootingHandler shootingHandler;
    ObjectPooling currentPooler;
    TowerContainer towerContainer;
    Tilemap tilemap;
    PathfindingManager pathfindingManager;

    GameObject rangeIndicator;

    TowerContainer.Tower baseTower;
    int moneySpent;
    float coolDown;
    float range;
    int damage;
    bool shootingEnabled;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
    }

    /// <summary>
    /// Activates the tower once it's purchase has been confirmed
    /// </summary>
    public void EnableTower()
    {
        shootingHandler.EnableTower();
        shootingEnabled = true;
    }
    void GetBaseTower()
    {
        shootingHandler = gameObject.GetComponent<ShootingHandler>();
        shootingEnabled = false;
        rangeIndicator = transform.Find("RangeIndicator").gameObject;
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        if (towerManagerObject != null)
        {
            towerSelectionHandler = towerManagerObject.GetComponent<TowerSelectionHandler>();
        }
        towerContainer = TowerContainer.getInstance();
        foreach (TowerContainer.Tower tower in towerContainer.towers)
        {
            if (gameObject.name.Contains(tower.name))
            {
                baseTower = tower;
                currentPooler = towerSelectionHandler.GetTowerPoolerFromBaseTower(baseTower);
                ResetBuffs();
                break;
            }
        }
    }
    public void ResetBuffs() 
    {
        if (baseTower == null)
        {
            GetBaseTower();
        }
        coolDown = baseTower.attackSpeed;
        range = baseTower.attackRange;
        damage = baseTower.attackDamage;
        moneySpent = baseTower.cost;
    }
    public void ApplyBuff(TileContainer.Tile tile)
    {
        range *= tile.attackRange;
        if (baseTower == null)
        {
            GetBaseTower();
        }
        shootingHandler.SetRange(range);
        UpdateRangeIndicator();
    }
    public void UpgradeTower()
    {
        Debug.Log("TODO: Implement upgrading");
    }
    public bool GetShootingState()
    {
        return shootingEnabled;
    }
    void UpdateRangeIndicator()
    {
        rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 0);
    }
    public void SellTower()
    {
        moneyHandler.AddMoney(GetSellCost());
        Vector3Int cellPosition = tilemap.WorldToCell(gameObject.transform.position);
        pathfindingManager.RemoveTowerFromNode(cellPosition);
        currentPooler.DeactivateObject(gameObject);
    }
    public void EnableRangeIndicator()
    {
        rangeIndicator.SetActive(true);
    }
    public void DisableRangeIndicator()
    {
        rangeIndicator.SetActive(false);
    }
    public int GetSellCost()
    {
        return moneySpent / 2;
    }
    public int GetAttackDamage()
    {
        return damage;
    }
    public float GetAttackRange()
    {
        return range;
    }
}
