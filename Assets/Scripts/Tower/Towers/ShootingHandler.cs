using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootingHandler : MonoBehaviour
{
    GameObject partToRotate;
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
            enemy.ReduceHealth(damage);
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
