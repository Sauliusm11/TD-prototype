using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Class attached to every tower object, responsible for handling upgrades and tower stats(power)
/// </summary>
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
    /// <summary>
    /// Pulls the default values of the chosen tower type,
    /// resets everything else to the defaults for that tower type
    /// </summary>
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
    /// <summary>
    /// Reset all stat buffs of the current tower, if no tower exists, get a new one
    /// </summary>
    public void ResetBuffs() 
    {
        if (baseTower == null)
        {
            GetBaseTower();
        }
        else 
        { 
            coolDown = baseTower.attackSpeed;
            range = baseTower.attackRange;
            damage = baseTower.attackDamage;
            moneySpent = baseTower.cost;
            projectileSpeed = baseTower.projectileSpeed;
            //This is where you reset tower looks
            UpdateBaseSprite(0);
            UpdateRotatingSprite(0);
        }
    }
    /// <summary>
    /// Called by tower placment. 
    /// Applies range buff based on the tile the tower is placed on
    /// </summary>
    /// <param name="tile">The tile that the tower is placed on</param>
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
    /// <summary>
    /// Handles tower upgrading
    /// </summary>
    /// <param name="primary">True - regular upgrade, false - skip a tier(for alternate elite only)</param>
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
    /// <summary>
    /// Check if the current tower is enabled(not in ghost form)
    /// </summary>
    /// <returns></returns>
    public bool GetShootingState()
    {
        return shootingEnabled;
    }
    /// <summary>
    /// Update the range indicator scale based on current range
    /// </summary>
    void UpdateRangeIndicator()
    {
        rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 0);
    }
    /// <summary>
    /// Replace the sprite of the tower base with the new tier sprite
    /// </summary>
    /// <param name="tier">Tier of the upgrade</param>
    void UpdateBaseSprite(int tier)
    {
        baseSpriteRenderer.sprite = tierSprites[tier];
    }
    /// <summary>
    /// Replace the sprite of the rotating tower part with the new tier sprite
    /// (only for elite upgrades)
    /// </summary>
    /// <param name="tier">Tier of the upgrade</param>
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
    /// <summary>
    /// Handles the selling of the tower (money, removing, etc.)
    /// </summary>
    public void SellTower()
    {
        moneyHandler.AddMoney(GetSellCost());
        Vector3Int cellPosition = tilemap.WorldToCell(gameObject.transform.position);
        pathfindingManager.RemoveTowerFromNode(cellPosition);
        shootingHandler.DisableTower();
        currentPooler.DeactivateObject(gameObject);
    }
    /// <summary>
    /// Get the sell cost of the tower
    /// </summary>
    /// <returns>Total money spent on tower / 2 </returns>
    public int GetSellCost()
    {
        return moneySpent / 2;
    }
    /// <summary>
    /// Check if an upgrade is available for the tower
    /// </summary>
    /// <returns>True if an upgrade can be afforded</returns>
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
    /// <summary>
    /// Get the upgrade object of the next upgrade tier
    /// </summary>
    /// <returns>If no more upgrades exist returns upgrade with tier set to -1 </returns>
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
    /// <summary>
    /// Get the upgrade object of the secondary elite upgrade
    /// </summary>
    /// <returns></returns>
    public TowerContainer.Upgrade GetSecondaryElite()
    {
        return upgradeTree[baseTower.maxTier-1];
    }
    public void EnableRangeIndicator()
    {
        rangeIndicator.SetActive(true);
    }
    public void DisableRangeIndicator()
    {
        rangeIndicator.SetActive(false);
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
