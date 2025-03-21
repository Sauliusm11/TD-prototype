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

    GameObject rangeIndicator;

    TowerContainer.Tower baseTower;
    List<TowerContainer.Upgrade> upgradeTree = new List<TowerContainer.Upgrade>();
    [SerializeField]
    List<Sprite> tierSprites = new List<Sprite>();
    SpriteRenderer spriteRenderer;
    int currentTier;
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
        currentTier = 0;
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
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
                upgradeTree = tower.upgrades;
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
        //This is where you reset tower looks
        UpdateBaseSprite(0);
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
        if (currentTier < baseTower.maxTier)
        {
            if (moneyHandler.RemoveMoney(upgradeTree[currentTier].cost)) 
            { 
                range *= 1+upgradeTree[currentTier].attackRange;
                UpdateRangeIndicator();
                damage += upgradeTree[currentTier].attackDamage;
                coolDown -= upgradeTree[currentTier].attackSpeed;

                //TODO: Projectile speed?

                currentTier++;
                UpdateBaseSprite(currentTier);
            }
        }

    }
    public bool GetShootingState()
    {
        return shootingEnabled;
    }
    void UpdateRangeIndicator()
    {
        rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 0);
    }
    void UpdateBaseSprite(int tier)
    {
        spriteRenderer.sprite = tierSprites[tier];
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
    public bool IsUpgradeAvailable()
    {
        if (currentTier < baseTower.maxTier)
        {
            return moneyHandler.HasEnoughMoney(upgradeTree[currentTier].cost);
        }
        else 
        { 
            return false;
        }
    }
    public TowerContainer.Upgrade GetUpgrade()
    {
        if (currentTier < baseTower.maxTier)
        {
            return upgradeTree[currentTier];
        }
        else
        {
            return new TowerContainer.Upgrade(-1,0,0,0,0,0);
        }
    }
    public int GetAttackDamage()
    {
        return damage;
    }
    public float GetAttackSpeed()
    {
        return 1 / coolDown;
    }
    public float GetAttackRange()
    {
        return range;
    }
}
