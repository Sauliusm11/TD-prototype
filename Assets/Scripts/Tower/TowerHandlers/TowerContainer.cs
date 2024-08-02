using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerContainer
{

    private static TowerContainer instance;
    private static object threadLock = new object();

    public List<Tower> towers = new List<Tower>();

    /// <summary>
    /// (Should be) The only way to gain access to the TowerContainer 
    /// </summary>
    /// <returns></returns>
    public static TowerContainer getInstance()
    {

        if (instance == null)
        {
            lock (threadLock)
            {
                if (instance == null)
                {
                    instance = new TowerContainer();
                }
            }
        }
        return instance;
    }
    TowerContainer()
    {
        instance = this;
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        towers = parser.LoadTowerList();
    }

    [System.Serializable]
    public class TowerList
    {
        public List<Tower> Towers = new List<Tower>();
    }
    [System.Serializable]
    public class Tower
    {
        public string name;
        public double attackSpeed;
        public int attackDamage;
        public double attackRange;
        public int cost;
        public Tower(string name, double attackSpeed, int attackDamage, double attackRange, int cost)
        {
            this.name = name;
            this.attackSpeed = attackSpeed;
            this.attackDamage = attackDamage;
            this.attackRange = attackRange;
            this.cost = cost;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
