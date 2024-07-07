using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonParser : MonoBehaviour
{
    [System.Serializable]
    internal class TileList
    {
        public List<TileInfo> Tiles = new List<TileInfo>();
    }
    [System.Serializable]
    internal class TileInfo
    {
        public string name;
        public float movementSpeed;
        public float damageResistance;
        public float attackRange;
    }
    TileList list = new TileList();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveLevelTiles()
    {

    }

    public List<TileContainer.Tile> LoadTileList() 
    {
        //return File.ReadAllText(Application.dataPath + "/TileMaps/Tiles/" + "TileData.json"/*filename*/);
        string json = File.ReadAllText(Application.dataPath + "/TileMaps/Tiles/" + "TileData.json"/*filename*/);
        list = JsonUtility.FromJson<TileList>(json);
        List<TileContainer.Tile> tiles = new List<TileContainer.Tile>();
        foreach (TileInfo tileInfo in list.Tiles) 
        {
            TileContainer.Tile tile = new TileContainer.Tile(tileInfo.name,tileInfo.movementSpeed,tileInfo.damageResistance,tileInfo.attackRange);
            tiles.Add(tile);
        }
        return tiles;
    }
}
