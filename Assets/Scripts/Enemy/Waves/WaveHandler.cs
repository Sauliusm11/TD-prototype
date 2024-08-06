using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    JsonParser parser;
    List<Wave> waves;
    bool sending;
    int currentWave;
    [SerializeField]
    GameObject enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        sending = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadWaves(string fileName)
    {
        currentWave = 0;
        waves = parser.LoadLevelWaves(fileName);
    }
    public void StartWave(Node start, float xOffset, float yOffset)
    {
        if (!sending && currentWave < waves.Count)
        {
            sending = true;
            StartCoroutine(SendWave(currentWave,start,xOffset,yOffset));
            Debug.Log(sending);
            currentWave++;
        }
    }
    IEnumerator SendWave(int waveNumber, Node start, float xOffset, float yOffset)
    {
        Wave wave = waves[waveNumber];
        List<WaveEnemy> enemies = wave.Enemies;
        while (enemies.Count > 0)
        {
            WaveEnemy enemy = enemies[0];
            while (enemy.count > 0) 
            {
                Instantiate(enemyPrefab, new Vector3(start.GetX() + xOffset, start.GetY() + yOffset, 0), new Quaternion());//+ offset to center the enemy on the tile
                enemy.count--;
                yield return new WaitForSeconds(0.1f);
            }
            enemies.RemoveAt(0);
        }
        sending = false;
    }
}
