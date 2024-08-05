using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContainer
{
    private static EnemyContainer instance;
    private static object threadLock = new object();

    public List<Enemy> enemies = new List<Enemy>();

    /// <summary>
    /// (Should be) The only way to gain access to the EnemyContainer 
    /// </summary>
    /// <returns></returns>
    public static EnemyContainer getInstance()
    {

        if (instance == null)
        {
            lock (threadLock)
            {
                if (instance == null)
                {
                    instance = new EnemyContainer();
                }
            }
        }
        return instance;
    }
    EnemyContainer()
    {
        instance = this;
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        enemies = parser.LoadEnemyList();
    }

    [System.Serializable]
    public class EnemyList
    {
        public List<Enemy> Enemies = new List<Enemy>();
    }
    [System.Serializable]
    public class Enemy
    {
        public string name;
        public int health;
        public int livesCost;
        public float speedCoef;
        public string type;
        public int reward;
        public Enemy(string name, int health, int livesCost, float speedCoef, string type,int reward)
        {
            this.name = name;
            this.health = health;
            this.livesCost = livesCost;
            this.speedCoef = speedCoef;
            this.type = type;
            this.reward = reward;
        }
    }
}
