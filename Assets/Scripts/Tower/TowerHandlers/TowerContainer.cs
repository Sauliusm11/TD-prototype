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
        public float attackSpeed;
        public int attackDamage;
        public float attackRange;
        public int cost;
        public float projectileSpeed;
        public int maxTier;
        public List<Upgrade> upgrades = new List<Upgrade>();
        public Tower(string name, float attackSpeed, int attackDamage, float attackRange, int cost, float projectileSpeed, List<Upgrade> upgrades, int maxTier)
        {
            this.name = name;
            this.attackSpeed = attackSpeed;
            this.attackDamage = attackDamage;
            this.attackRange = attackRange;
            this.cost = cost;
            this.projectileSpeed = projectileSpeed;
            this.upgrades = upgrades;
            this.maxTier = maxTier;
        }
    }
    [System.Serializable]
    public class Upgrade
    {
        public int tier;
        public float attackSpeed;
        public int attackDamage;
        public float attackRange;
        public int cost;
        public float projectileSpeed;
        public Upgrade(int tier, float attackSpeed, int attackDamage, float attackRange, int cost, float projectileSpeed)
        {
            this.tier = tier;
            this.attackSpeed = attackSpeed;
            this.attackDamage = attackDamage;
            this.attackRange = attackRange;
            this.cost = cost;
            this.projectileSpeed = projectileSpeed;
        }
    }
}
