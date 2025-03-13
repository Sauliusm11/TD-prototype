using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
/// <summary>
/// Wave handler class responsible for managing waves and spawning enemies
/// Should only be attached to the WaveManager object
/// </summary>
public class WaveHandler : MonoBehaviour
{
    GameManager gameManager;
    JsonParser parser;
    List<Wave> waves;
    bool sending;
    int currentWave;
    List<ObjectPooling> enemyPoolers;
    ObjectPooling currentEnemyPooler;
    [SerializeField]
    float maxOffset;
    int enemyCount;
    bool waveFinished;
    // Start is called before the first frame update
    void Start()
    {
        enemyPoolers = new List<ObjectPooling>
        {
            GameObject.Find("BasicEnemyPooler").GetComponent<ObjectPooling>(),
            GameObject.Find("BasicNoTerrainEnemyPooler").GetComponent<ObjectPooling>()
        };
        parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        sending = false;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Loads the wave information
    /// </summary>
    /// <param name="fileName">Name of the level wave file</param>
    public void LoadWaves(string fileName)
    {
        currentWave = 0;
        waves = parser.LoadLevelWaves(fileName);
        waveFinished = true;
    }
    /// <summary>
    /// Starts the next wave if the current wave is finished
    /// Should only be called by PathFindingManager once all the pathfinders have finished
    /// </summary>
    /// <param name="start">Node object of the portal</param>
    /// <param name="xOffset">xOffset value saved in the PathfindingManager</param>
    /// <param name="yOffset">yOffset value saved in the PathfindingManager</param>
    public void StartWave(Node start, float xOffset, float yOffset)
    {
        if (waveFinished && !sending && currentWave < waves.Count)//Could this be a race condition?
        {
            sending = true;
            waveFinished = false;
            gameManager.WaveStarted(currentWave,waves.Count);
            StartCoroutine(SendWave(currentWave,start,xOffset,yOffset));
            currentWave++;
        }
    }
    public void DecreaseEnemyCount()
    {
        enemyCount--;
        if (enemyCount == 0 && !sending)
        {
            gameManager.WaveEnded();
            waveFinished=true;
        }
    }
    /// <summary>
    /// Coroutine responsible for spawning the enemies of a wave
    /// </summary>
    /// <param name="waveNumber">Number of the wave</param>
    /// <param name="start">Starting node</param>
    /// <param name="xOffset">base xOffset value saved in the PathfindingManager</param>
    /// <param name="yOffset">base yOffset value saved in the PathfindingManager</param>
    /// <returns></returns>
    IEnumerator SendWave(int waveNumber, Node start, float xOffset, float yOffset)
    {
        enemyCount = 0;
        Wave wave = waves[waveNumber];
        List<WaveEnemy> enemies = wave.Enemies;
        while (enemies.Count > 0)
        {
            WaveEnemy enemy = enemies[0];
            string enemyName = enemy.name;
            foreach (ObjectPooling pooler in enemyPoolers)
            {
                if (pooler.gameObject.name.Contains(enemyName))
                {
                    currentEnemyPooler = pooler;
                    Debug.Log(pooler.gameObject.name);
                }
            }
            while (enemy.count > 0) 
            {
                //Need to add some variance to enemies(but also their target then)(prob stored in the enemy itself)
                float additionalXOffset = Random.Range(-maxOffset, maxOffset);
                float additionalYOffset = Random.Range(-maxOffset, maxOffset);
                GameObject enemyObject = currentEnemyPooler.ActivateObject(new Vector3(start.GetX() + xOffset + additionalXOffset, start.GetY() + yOffset + additionalYOffset, 1), new Quaternion());
                BaseEnemy baseEnemy = enemyObject.GetComponent<BaseEnemy>();
                baseEnemy.UpdateOffsets(additionalXOffset, additionalYOffset);
                baseEnemy.StartWalking();
                enemy.count--;
                enemyCount++;
                yield return new WaitForSeconds(0.2f);
            }
            enemies.RemoveAt(0);
        }
        sending = false;
    }
}
