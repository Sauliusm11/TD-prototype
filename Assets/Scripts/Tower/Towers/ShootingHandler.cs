using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Shooting handler class attached to each tower (TODO: consider making this abstract?)
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

    float coolDown;
    float range;
    int damage;
    bool enabled;
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
                coolDown = tower.attackSpeed;
                range = tower.attackRange;
                damage = tower.attackDamage;
            }
        }
        timeSinceShot = coolDown;
        enabled = false;
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
                coolDown = tower.attackSpeed;
                range = tower.attackRange;
                damage = tower.attackDamage;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Ehm, so it resets the cooldown even when no targets?
        if (enabled) 
        { 
            timeSinceShot += Time.deltaTime;
            if(timeSinceShot > coolDown) 
            {
                Shoot();
            }
        }
    }
    /// <summary>
    /// Apply stat changes to the tower based on the given tile. Should be called when the tower is finally placed
    /// </summary>
    /// <param name="tile"></param>
    public void ApplyBuff(TileContainer.Tile tile)
    {
        range *= tile.attackRange;
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
            //GameObject bullet = Instantiate(bulletPrefab, shootingPoint.transform);//This REALLY needs object pooling
            GameObject bullet = bulletPooler.ActivateObject(shootingPoint.transform);//TODO: this is not ideal, would be better to give it the prefab(allowing one pooler to have different objects)
            StartCoroutine(MoveBulletTo(targetObject, bullet, 5));
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
        if (!enabled) 
        { 
            enabled = true;
            InvokeRepeating("AimAtTarget", 0, 0.05f);//Increase the last number in case of performance issues(makes aiming choppier)
        }
    }
    /// <summary>
    /// Coroutine which moves the bullet to the target
    /// </summary>
    /// <param name="target">Object the bullet is targeting</param>
    /// <param name="bullet">Bullet object</param>
    /// <param name="speed">Speed at which the bullet moves (should always be faster than fastest enemy)</param>
    /// <returns></returns>
    IEnumerator MoveBulletTo(GameObject target, GameObject bullet, float speed)
    {
        Vector3 oldPos = bullet.transform.position;
        Vector3 goTo;
        if (target != null)
        {
            goTo = target.transform.position;
            Vector3 path = oldPos - goTo;//This is close but not quite right (I might need to flip what is left over after this operation)
            path *= -1;//Flipping because we are -1 away from destination
            float totalTime = 1f;//Time in seconds at which a base speed unit crosses a base speed tile.
            if (speed != 0f)
            {
                totalTime /= speed;
                float timeElapsed = Time.deltaTime;
                while (timeElapsed < totalTime)
                {
                    if (target != null)
                    {
                        goTo = target.transform.position;
                    }
                    else
                    {
                        bulletPooler.DeactivateObject(bullet);
                        //Destroy(bullet);
                        break;
                    }
                    float timeDelta = Time.deltaTime;
                    timeElapsed += timeDelta;
                    bullet.transform.position += path * (timeDelta / totalTime);
                    if (timeElapsed / totalTime > 1)
                    {
                        bullet.transform.position = goTo;
                        bulletPooler.DeactivateObject(bullet);
                        //Destroy(bullet);
                        BaseEnemy enemy = target.GetComponent<BaseEnemy>();
                        enemy.ReduceHealth(damage);

                    }
                    yield return null;
                }
            }
        }
        else
        {
            bulletPooler.DeactivateObject(bullet);
            //Destroy(bullet);
        }

    }
}
