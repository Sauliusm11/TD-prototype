using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class for the basic movement enemy type (might become abstract later :p)
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    Money moneyHandler;
    EnemyPathFinding pathFinder;
    Stack<WorldNode> path;
    float timeElapsed = 0f;
    float totalTime = 1f;//Time in seconds at which a base speed unit crosses a base speed tile.
    int maxHealth;
    int currentHealth;
    float walkingSpeed;
    int livesCost;
    [SerializeField]
    GameObject healthBarPrefab;
    GameObject healthBarParent;
    GameObject healthBarObject;
    Slider healthBarSlider;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: again, object pooling is waiting
        healthBarParent = GameObject.Find("UIWorldSpaceCanvas");
        healthBarObject = Instantiate(healthBarPrefab,healthBarParent.transform);
        UpdateHealthBarPosition();
        healthBarSlider = healthBarObject.GetComponent<Slider>();
        EnemyContainer enemyContainer = EnemyContainer.getInstance();
        foreach (EnemyContainer.Enemy enemy in enemyContainer.enemies)
        {
            if (this.name.Contains(enemy.name))
            {
                maxHealth = enemy.health;
                currentHealth = enemy.health;
                walkingSpeed = enemy.speedCoef;
                livesCost = enemy.livesCost;
            }
        }
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        pathFinder = GameObject.Find("BasicEnemyPathfinder").GetComponent<EnemyPathFinding>();
        path = pathFinder.GetPath();
        StartCoroutine(FollowPath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float GetProgress()
    {
        if (timeElapsed != 0)
        {
            return path.Count + totalTime / timeElapsed;
        }
        return path.Count;
    }
    public void ReduceHealth(int damage)
    {
        currentHealth -= damage;
        healthBarSlider.value = (float)currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            moneyHandler.AddMoney(10);
            Destroy(healthBarObject);
            Destroy(gameObject);
        }
    }
    void UpdateHealthBarPosition() 
    {
        Vector3 position = gameObject.transform.position;
        healthBarObject.transform.position = new Vector3(position.x, position.y+0.3f, position.z);
    }
    /// <summary>
    /// Move the enemy one tile at a time along the path
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        while (path.Count > 0)
        {
            WorldNode node = path.Pop();
            yield return StartCoroutine(MoveTo(node.GetVector3(), node.GetMovementSpeedCoef()*walkingSpeed));
        }
        yield return null;
    }
    /// <summary>
    /// Increment the movement of the enemy to a given world space
    /// </summary>
    /// <param name="goTo">End position of the enemy</param>
    /// <param name="speed">Speed at which the enemy 'walks'</param>
    /// <returns></returns>
    IEnumerator MoveTo(Vector3 goTo, float speed)
    {
        GameObject enemy = gameObject;
        Vector3 oldPos = enemy.transform.position;
        Vector3 path = oldPos - goTo;//This is close but not quite right (I might need to flip what is left over after this operation)
        path *= -1;//Flipping because we are -1 away from destination
        totalTime = 1f;//Time in seconds at which a base speed unit crosses a base speed tile.
        if (speed != 0f)
        {
            totalTime /= speed;
            timeElapsed = 0.000000000001f;
            while (timeElapsed < totalTime)
            {
                float timeDelta = Time.deltaTime;
                timeElapsed += timeDelta;
                enemy.transform.position += path * (timeDelta / totalTime);
                if (timeElapsed / totalTime > 1)
                {
                    enemy.transform.position = goTo;
                }
                UpdateHealthBarPosition();
                yield return null;
            }
        }
    }
}
