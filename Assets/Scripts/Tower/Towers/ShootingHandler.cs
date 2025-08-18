using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shooting handler class attached to each tower
/// </summary>
public class ShootingHandler : MonoBehaviour
{
    GameObject partToRotate;
    [SerializeField]
    GameObject bulletPrefab;
    GameObject shootingPoint;
    List<GameObject> EnemyObjects = new List<GameObject>();
    List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
    int currentTarget;
    float timeSinceShot;
    ObjectPooling bulletPooler;

    float cooldown;
    float range;
    int damage;
    float projectileSpeed;
    bool shootingEnabled;
    // Start is called before the first frame update
    void Start()
    {
        bulletPooler = GameObject.Find("CannonBulletPooler").GetComponent<ObjectPooling>();
        partToRotate = transform.Find("TowerCannon").gameObject;
        shootingPoint = partToRotate.transform.Find("ShootingPoint").gameObject;
        TowerContainer towerContainer = TowerContainer.getInstance();
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
        timeSinceShot = cooldown;
        shootingEnabled = false;
    }
    /// <summary>
    /// Because of object pooling this is needed to set certain attributes back to default
    /// </summary>
    private void OnDisable()
    {
        TowerContainer towerContainer = TowerContainer.getInstance();
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
    void Shoot()
    {
        //Update target right before shooting
        AimAtTarget();
        if (currentTarget > -1)
        {
            GameObject targetObject = EnemyObjects[currentTarget];
            GameObject bullet = bulletPooler.ActivateObject(shootingPoint.transform);
            bullet.GetComponent<BulletHandler>().ShootBullet(targetObject, bullet, projectileSpeed, damage);
            timeSinceShot = 0;
        }
    }
    /// <summary>
    /// Finds the enemy cloest to the exit within range and points the part to rotate at it.
    /// </summary>
    void AimAtTarget()
    {
        //Update target list before picking target
        UpdateTargets();
        float closestToExit = float.MaxValue;//Flip for last
        int closestIndex = 0;
        if (baseEnemies.Count > 0)
        {
            for (int i = 0; i < baseEnemies.Count; i++)
            {
                float progress = baseEnemies[i].GetProgress();
                if (progress < closestToExit)//Flip for last
                {
                    closestIndex = i;
                    closestToExit = progress;
                }
            }
            currentTarget = closestIndex;
            GameObject targetObject = EnemyObjects[currentTarget];
            //Rotates the gun to point at the target
            Vector2 direction = partToRotate.transform.position - targetObject.transform.position;
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90);//Idk why, but + 90 helps
            partToRotate.transform.rotation = rotation;
        }
        else
        {
            currentTarget = -1;
        }

    }
    /// <summary>
    /// Uses a physics2D OverlapCircleAll to find all enemies within range
    /// </summary>
    void UpdateTargets()
    {
        //Change the second one to 2 for ground+air guns and both for air only guns?
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, range, 1 << 6, 1, 1);
        //There has to be a better way, right?
        EnemyObjects.Clear();
        baseEnemies.Clear();
        foreach (Collider2D collider in colliders)
        {
            GameObject collisionGameObject = collider.gameObject;
            if (!EnemyObjects.Contains(collisionGameObject))
            {
                BaseEnemy baseEnemy = collisionGameObject.GetComponent<BaseEnemy>();
                if (baseEnemy != null)
                {
                    EnemyObjects.Add(collisionGameObject);
                    baseEnemies.Add(baseEnemy);
                }
            }
        }
    }
    /// <summary>
    /// Activates the tower once it's purchase has been confirmed
    /// </summary>
    public void EnableTower()
    {
        if (!shootingEnabled)
        {
            shootingEnabled = true;
            InvokeRepeating("AimAtTarget", 0, 0.05f);//Increase the last number in case of performance issues(makes aiming choppier)
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
