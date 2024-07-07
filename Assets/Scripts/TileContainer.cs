using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class TileContainer 
{
    private static TileContainer instance;
    private static object threadLock = new object();

    //public List<Tile> tiles = new List<Tile>();
    public List<Tile> tiles = new List<Tile>();

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
        JsonParser parser = new JsonParser();
        tiles = parser.LoadTileList();
        Debug.Log(tiles[3].name);
    }
    //public void PrintTiles()
    //{
    //    this.tiles
    //}
    [System.Serializable]
    public class TileList
    {
        public List<Tile> tiles = new List<Tile>();
    }
    [System.Serializable]
    public class Tile
    {
        public string name;
        public double movementSpeed;
        public double damageResistance;
        public double attackRange;

        public Tile(string name, float movementSpeed, float damageResistance, float attackRange)
        {
            this.name = name;
            this.movementSpeed = movementSpeed;
            this.damageResistance = damageResistance;
            this.attackRange = attackRange;
        }

    }
}
