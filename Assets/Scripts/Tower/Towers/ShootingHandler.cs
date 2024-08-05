using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    float coolDown;
    float range;
    int damage;
    // Start is called before the first frame update
    void Start()
    {
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
        timeSinceShot = 0;
        InvokeRepeating("AimAtTarget", 0, 0.05f);//Increase the last number in case of performance issues(makes aiming choppier)
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceShot += Time.deltaTime;
        if(timeSinceShot > coolDown) 
        {
            timeSinceShot = 0;
            Shoot();
        }
    }
    void Shoot()
    {
        AimAtTarget();
        if (currentTarget > -1)
        {

            GameObject targetObject = EnemyObjects[currentTarget];
            BaseEnemy enemy = targetObject.GetComponent<BaseEnemy>();
            //enemy.ReduceHealth(damage);
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.transform);//This REALLY needs object pooling
            StartCoroutine(MoveBulletTo(targetObject, bullet, 5));
            Debug.Log("Shoot?");
        }
    }
    void AimAtTarget() 
    {
        UpdateTargets();
        float closestToExit = float.MinValue;
        int closestIndex = 0;
        if (baseEnemies.Count > 0)
        {
            for (int i = 0; i < baseEnemies.Count; i++)
            {
                if (baseEnemies[i].GetProgress() > closestToExit)
                {
                    closestIndex = i;
                }
            }
            currentTarget = closestIndex;
            GameObject targetObject = EnemyObjects[currentTarget];
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
    void UpdateTargets()
    {
        //Change the second one to 2 for ground+air guns and both for air only guns
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
                        Destroy(bullet);
                        break;
                    }
                    float timeDelta = Time.deltaTime;
                    timeElapsed += timeDelta;
                    bullet.transform.position += path * (timeDelta / totalTime);
                    if (timeElapsed / totalTime > 1)
                    {
                        bullet.transform.position = goTo;
                        Destroy(bullet);
                        BaseEnemy enemy = target.GetComponent<BaseEnemy>();
                        enemy.ReduceHealth(damage);

                    }
                    yield return null;
                }
            }
        }
        else
        {
            Destroy(bullet);
        }
        
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("At least I collided");
    //    GameObject collisionGameObject = collision.gameObject;
    //    if (!EnemyObjects.Contains(collisionGameObject))
    //    {
    //        BaseEnemy baseEnemy = collisionGameObject.GetComponent<BaseEnemy>();
    //        if(baseEnemy != null) 
    //        { 
    //            EnemyObjects.Add(collision.gameObject);
    //            baseEnemies.Add(baseEnemy);
    //            Debug.Log("At least I added");
    //        }
    //    }
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    GameObject collisionGameObject = collision.gameObject;
    //    if (EnemyObjects.Contains(collisionGameObject))
    //    {
    //        BaseEnemy baseEnemy = collisionGameObject.GetComponent<BaseEnemy>();
    //        if (baseEnemy != null)
    //        {
    //            EnemyObjects.Remove(collision.gameObject);
    //            baseEnemies.Remove(baseEnemy);
    //        }
    //    }
    //}
}
