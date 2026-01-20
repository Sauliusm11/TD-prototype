using Assets.Scripts.Tower.Towers.ShootingHandlers.TargetingTypes;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shooting handler class attached to each tower
/// </summary>
public abstract class ShootingHandler : MonoBehaviour
{
    protected GameObject partToRotate;
    [SerializeField]
    GameObject bulletPrefab;
    protected GameObject shootingPoint;
    protected TargetingHandler targetingHandler;
    protected List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
    protected int currentTarget;
    protected float timeSinceShot;
    protected ObjectPooling bulletPooler;

    protected float cooldown;
    protected float range;
    protected int damage;
    protected float projectileSpeed;
    bool shootingEnabled;

    private void OnEnable()
    {
        timeSinceShot = cooldown;
        shootingEnabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        targetingHandler = GetComponent<TargetingHandler>();
        bulletPooler = GameObject.Find("CannonBulletPooler").GetComponent<ObjectPooling>();
        partToRotate = transform.Find("TowerCannon").gameObject;
        shootingPoint = partToRotate.transform.Find("ShootingPoint").gameObject;
        TowerContainer towerContainer = TowerContainer.GetInstance();
        foreach (TowerContainer.Tower tower in towerContainer.towers)
        {
            if (this.name.Contains(tower.name))
            {
                cooldown = 1 / tower.attackSpeed;
                range = tower.attackRange;
                damage = tower.attackDamage;
                projectileSpeed = tower.projectileSpeed;
            }
        }
    }
    /// <summary>
    /// Because of object pooling this is needed to set certain attributes back to default
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
        TowerContainer towerContainer = TowerContainer.GetInstance();
        foreach (TowerContainer.Tower tower in towerContainer.towers)
        {
            if (this.name.Contains(tower.name))
            {
                cooldown = 1 / tower.attackSpeed;
                range = tower.attackRange;
                damage = tower.attackDamage;
                projectileSpeed = tower.projectileSpeed;
            }
        }
        if (partToRotate != null)
        {
            Quaternion rotation = new Quaternion();
            partToRotate.transform.rotation = rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shootingEnabled)
        {
            timeSinceShot += Time.deltaTime;
            if (timeSinceShot > cooldown)
            {
                Shoot();
            }
        }
    }
    /// <summary>
    /// Called internally when the shot cooldown has reset
    /// </summary>
    public abstract void Shoot();

    /// <summary>
    /// Finds the enemy cloest to the exit within range and points the part to rotate at it.
    /// </summary>
    public abstract void AimAtTarget();

    /// <summary>
    /// Activates the tower once it's purchase has been confirmed
    /// </summary>
    public void EnableTower()
    {
        if (!shootingEnabled)
        {
            shootingEnabled = true;
            InvokeRepeating(nameof(AimAtTarget), 0, 0.05f);//Increase the last number in case of performance issues(makes aiming choppier)
        }
    }
    /// <summary>
    /// Activates the tower once it's selling has been confirmed
    /// </summary>
    public void DisableTower()
    {
        if (shootingEnabled)
        {
            shootingEnabled = false;
        }
    }
    /// <summary>
    /// Update the actual range with the new value, should only be called by upgrade handler
    /// </summary>
    /// <param name="newRange">New range value</param>
    public void SetRange(float newRange)
    {
        range = newRange;
    }
    /// <summary>
    /// Update the actual damage with the new value, should only be called by upgrade handler
    /// </summary>
    /// <param name="newDamage">New damage value</param>
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }
    /// <summary>
    /// Update the actual cooldown with the new value, should only be called by upgrade handler
    /// </summary>
    /// <param name="newCooldown">New cooldown value</param>
    public void SetCooldown(float newCooldown)
    {
        cooldown = newCooldown;
    }
    /// <summary>
    /// Update the actual projectile speed with the new value, should only be called by upgrade handler
    /// </summary>
    /// <param name="newProjectileSpeed">New projectile speed value</param>
    public void SetProjectileSpeed(float newProjectileSpeed)
    {
        projectileSpeed = newProjectileSpeed;
    }
}
