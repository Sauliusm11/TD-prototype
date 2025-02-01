using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class for the basic movement enemy type (might become abstract later :p)
/// </summary>
public abstract class BaseEnemy : MonoBehaviour
{
    Money moneyHandler;
    Lives livesHandler;
    EnemyPathFinding pathFinder;
    Stack<WorldNode> path;
    TileContainer tileContainer;
    TileContainer.Tile currentTile;
    float timeElapsed = 0f;
    float totalTime = 1f;//Time in seconds at which a base speed unit crosses a base speed tile.
    float xOffset;
    float yOffset;
    int maxHealth;
    int currentHealth;
    float walkingSpeed;
    int livesCost;
    int reward;
    [SerializeField]
    GameObject healthBarPrefab;
    GameObject healthBarParent;
    GameObject healthBarObject;
    Slider healthBarSlider;
    ObjectPooling enemyPooler;
    ObjectPooling enemyHealthBarPooler;
    WaveHandler waveHandler;
    // Start is called before the first frame update
    void Start()
    {
        enemyPooler = getEnemyPooler();
        waveHandler = GameObject.Find("WaveManager").GetComponent<WaveHandler>();
    }
    protected abstract ObjectPooling getEnemyPooler();

    /// <summary>
    /// Because of object pooling the same object can be 'created' multiple times
    /// </summary>
    private void OnEnable()
    {
        tileContainer = TileContainer.getInstance();
        enemyHealthBarPooler = GameObject.Find("EnemyHealthBarPooler").GetComponent<ObjectPooling>();
        healthBarParent = GameObject.Find("UIWorldSpaceCanvas");
        healthBarObject = enemyHealthBarPooler.ActivateObjectWithParent(healthBarParent.transform);
        healthBarObject.transform.SetAsFirstSibling();
        healthBarSlider = healthBarObject.GetComponent<Slider>();
        healthBarSlider.value = 1;
        UpdateHealthBarPosition();
        EnemyContainer enemyContainer = EnemyContainer.getInstance();
        foreach (EnemyContainer.Enemy enemy in enemyContainer.enemies)
        {
            if (this.name.Contains(enemy.name))
            {
                maxHealth = enemy.health;
                currentHealth = enemy.health;
                walkingSpeed = enemy.speedCoef;
                livesCost = enemy.livesCost;
                reward = enemy.reward;
                pathFinder = GameObject.Find(enemy.type + "Pathfinder").GetComponent<EnemyPathFinding>();
            }
        }
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        livesHandler = GameObject.Find("LivesHandler").GetComponent<Lives>();

    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// Gets the path from it's pathfinding manager and starts the follow path coroutine
    /// Should only be called from wave manager
    /// </summary>
    public void StartWalking()
    {
        path = pathFinder.GetPath();
        StartCoroutine(FollowPath());
    }
    /// <summary>
    /// Update the offset from the center of the tile, needed to make the path be equal length for all enemies while not walking in a single line
    /// Should only be called from wave manager
    /// </summary>
    /// <param name="X">X coordinate offset</param>
    /// <param name="Y">Y coordinate offset</param>
    public void UpdateOffsets(float X, float Y)
    {
        xOffset = X;
        yOffset = Y;
    }
    /// <summary>
    /// Calculates how far away from the castle is the enemy
    /// </summary>
    /// <returns>Distance in tiles?(or time?) from the castle</returns>
    public float GetProgress()
    {
        if (timeElapsed != 0)
        {
            return path.Count + 1 * (1 - timeElapsed / totalTime);
        }
        return path.Count + 1;
    }
    /// <summary>
    /// Damage the enemy and do enemy death related checks
    /// </summary>
    /// <param name="damage">Amount of damage to deal</param>
    public void ReduceHealth(int damage)
    {
        //TODO: Probably safer to use proper locks
        if (currentHealth > 0)
        {
            currentHealth -= Mathf.RoundToInt(damage * currentTile.damageMultiplier);
            healthBarSlider.value = (float)currentHealth / maxHealth;
            if (currentHealth <= 0)
            {
                moneyHandler.AddMoney(reward);
                Death();
            }
        }
    }
    /// <summary>
    /// Move the health bar attached to the enemy to position
    /// </summary>
    void UpdateHealthBarPosition()
    {
        Vector3 position = gameObject.transform.position;
        healthBarObject.transform.position = new Vector3(position.x, position.y + 0.3f, position.z);
    }
    /// <summary>
    /// Method called once the enemy health reaches 0
    /// </summary>
    void Death()
    {
        waveHandler.DecreaseEnemyCount();
        enemyHealthBarPooler.DeactivateObject(healthBarObject);
        enemyPooler.DeactivateObject(gameObject);
    }
    protected abstract float GetMoveSpeed(WorldNode node, float walkingSpeed);
    /// <summary>
    /// Move the enemy one tile at a time along the path
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        path.Pop();//Don't need to go to the portal
        while (path.Count > 0)
        {
            WorldNode node = path.Pop();
            string nodeName = node.GetName();
            foreach (TileContainer.Tile tile in tileContainer.tiles)
            {
                if (tile.name.Equals(nodeName))
                {
                    currentTile = tile;
                }
            }
            Vector3 target = node.GetVector3();
            target.x += xOffset;
            target.y += yOffset;
            yield return StartCoroutine(MoveTo(target, GetMoveSpeed(node, walkingSpeed)));
        }
        livesHandler.RemoveLives(livesCost);
        Death();
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
