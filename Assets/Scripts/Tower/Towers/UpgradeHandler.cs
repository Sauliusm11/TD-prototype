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
    Sprite defaultTowerSprite;
    [SerializeField]
    List<Sprite> tierSprites = new List<Sprite>();
    SpriteRenderer baseSpriteRenderer;
    SpriteRenderer rotatingSpriteRenderer;
    int currentTier;
    int moneySpent;
    float coolDown;
    float range;
    int damage;
    float projectileSpeed;
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
        baseSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        rotatingSpriteRenderer = gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
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
        projectileSpeed = baseTower.projectileSpeed;
        //This is where you reset tower looks
        UpdateBaseSprite(0);
        UpdateRotatingSprite(0);
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
    public void UpgradeTower(bool primary)
    {
        if (currentTier < baseTower.maxTier)
        {
            if (!primary)
            {
                currentTier++;
            }
            if (moneyHandler.RemoveMoney(upgradeTree[currentTier].cost)) 
            {
                moneySpent += upgradeTree[currentTier].cost;
                range *= 1+upgradeTree[currentTier].attackRange;
                UpdateRangeIndicator();
                shootingHandler.SetRange(range);
                damage += upgradeTree[currentTier].attackDamage;
                shootingHandler.SetDamage(damage);
                coolDown -= upgradeTree[currentTier].attackSpeed;
                shootingHandler.SetCooldown(coolDown);
                //TODO: Projectile speed?
                projectileSpeed += upgradeTree[currentTier].projectileSpeed;
                shootingHandler.SetProjectileSpeed(projectileSpeed);
                currentTier++;
                if( currentTier >= 3)
                {
                    UpdateRotatingSprite(currentTier);
                }
                else 
                { 
                    UpdateBaseSprite(currentTier);
                }
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
        baseSpriteRenderer.sprite = tierSprites[tier];
    }
    void UpdateRotatingSprite(int tier)
    {
        if (tier == 0)
        {
            rotatingSpriteRenderer.sprite = defaultTowerSprite;
        }
        else
        {
            rotatingSpriteRenderer.sprite = tierSprites[tier];
        }
    }
    public void SellTower()
    {
        moneyHandler.AddMoney(GetSellCost());
        Vector3Int cellPosition = tilemap.WorldToCell(gameObject.transform.position);
        pathfindingManager.RemoveTowerFromNode(cellPosition);
        shootingHandler.DisableTower();
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
    public TowerContainer.Upgrade GetSecondaryElite()
    {
        return upgradeTree[baseTower.maxTier-1];
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
