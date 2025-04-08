using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Singleton holding information about the different in game tile types
/// </summary>
public class TileContainer 
{
    private static TileContainer instance;
    private static object threadLock = new object();

    public List<Tile> tiles = new List<Tile>();

    /// <summary>
    /// (Should be) The only way to gain access to the TileContainer 
    /// </summary>
    /// <returns></returns>
    public static TileContainer getInstance()
    {

        if (instance == null)
        {
            lock (threadLock)
            {
                if (instance == null)
                {
                    instance = new TileContainer();
                }
            }
        }
        return instance;
    }
    TileContainer()
    {
        instance = this;
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        tiles = parser.LoadTileList();
    }
    /// <summary>
    /// List of tile types used by jsonParser to gather and return values of each tile type
    /// </summary>
    [System.Serializable]
    public class TileList
    {
        public List<Tile> tiles = new List<Tile>();
    }
    /// <summary>
    /// Tile class(not built-in) holding the stats of a tile type
    /// </summary>
    [System.Serializable]
    public class Tile
    {
        public string name;
        public float movementSpeed;
        public float damageMultiplier;
        public float attackRange;

        public Tile(string name, float movementSpeed, float damageMultiplier, float attackRange)
        {
            this.name = name;
            this.movementSpeed = movementSpeed;
            this.damageMultiplier = damageMultiplier;
            this.attackRange = attackRange;
        }

    }
}
