using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;


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
    TileList tileList = new TileList();
    [System.Serializable]
    class SavedTiles
    {
        public List<int> x = new List<int>();
        public List<int> y = new List<int>();
        public List<string> Name = new List<string>();
    }
    [System.Serializable]
    internal class TowerList
    {
        public List<TowerInfo> Towers = new List<TowerInfo>();
    }
    [System.Serializable]
    internal class TowerInfo
    {
        public string name;
        public float attackSpeed;
        public int attackDamage;
        public float attackRange;
        public int cost;
    }
    TowerList towerList = new TowerList();
    [System.Serializable]
    class SavedTowers
    {
        public List<int> x = new List<int>();
        public List<int> y = new List<int>();
        public List<string> Name = new List<string>();
    }

    Tilemap tilemap;
    TileSelectionHandler tileSelectionHandler;
    PathfindingManager pathfindingManager;
    void Start()
    {
        tileSelectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
    }
    /// <summary>
    /// Saves the current tile arrangement into a json file
    /// </summary>
    /// <param name="filename">Name of the file</param>
    public void SaveLevelTiles(string filename)
    {
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        SavedTiles savedTiles = new SavedTiles();
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                savedTiles.x.Add(x + bounds.position.x);
                savedTiles.y.Add(y + bounds.position.y);
                if (tile != null)
                {
                    savedTiles.Name.Add(tile.name);
                }
                else
                {
                    savedTiles.Name.Add("empty");
                }
            }
        }
        string jsonData = JsonUtility.ToJson(savedTiles);
        File.WriteAllText(Application.dataPath + "/Levels/" + filename, jsonData);
    }
    /// <summary>
    /// Loads the tile arrangement for a level from a Json file
    /// </summary>
    /// <param name="filename">Name of the file without the extention</param>
    public void LoadLevelTiles(string filename)
    {
        //Android(and build) version
        //TextAsset file = Resources.Load("Levels/" + filename) as TextAsset;
        //SavedTiles savedTiles = JsonUtility.FromJson<SavedTiles>(file.ToString());

        //Editor version
        string jsonData = File.ReadAllText(Application.dataPath + "/Levels/" + filename+".json");
        SavedTiles savedTiles = JsonUtility.FromJson<SavedTiles>(jsonData);
        
        List<Tile> Tiles = tileSelectionHandler.GetTileList();
        //Clear out current level
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x + bounds.position.x, y + bounds.position.y), null);
            }
        }
        //Loading the level
        for (int i = 0; i < savedTiles.Name.Count; i++)
        {
            string name = savedTiles.Name[i];
            if (name != "empty")
            {
                foreach (Tile tile in Tiles)
                {
                    if (tile.name.Equals(name))
                    {
                        tilemap.SetTile(new Vector3Int(savedTiles.x[i], savedTiles.y[i]), tile);
                    }
                }
            }
            else
            {
                tilemap.SetTile(new Vector3Int(savedTiles.x[i], savedTiles.y[i]), null);
            }
        }
        bounds = tilemap.cellBounds;
        pathfindingManager.LoadLevelTileList(tilemap.GetTilesBlock(bounds), bounds.size);
    }
    /// <summary>
    /// Loads all unique tiles and their values from the TileData.json file.
    /// Should only be called by the TileContainer
    /// </summary>
    /// <returns>A list of tile types used by TileContainer to store values for each tile type</returns>
    public List<TileContainer.Tile> LoadTileList() 
    {
        //Android(and build) version
        //TextAsset file = Resources.Load("TileMaps/Tiles/" + "TileData") as TextAsset;
        //tileList = JsonUtility.FromJson<TileList>(file.ToString());

        //Editor version
        string json = File.ReadAllText(Application.dataPath + "/TileMaps/Tiles/" + "TileData.json");
        tileList = JsonUtility.FromJson<TileList>(json);

        List<TileContainer.Tile> tiles = new List<TileContainer.Tile>();
        foreach (TileInfo tileInfo in tileList.Tiles) 
        {
            TileContainer.Tile tile = new TileContainer.Tile(tileInfo.name,tileInfo.movementSpeed,tileInfo.damageResistance,tileInfo.attackRange);
            tiles.Add(tile);
        }
        return tiles;
    }
    public List<TowerContainer.Tower> LoadTowerList()
    {
        //Android(and build) version
        //TextAsset file = Resources.Load("/Towers/" + "TowerData") as TextAsset;
        //towerList = JsonUtility.FromJson<TowerList>(file.ToString());

        //Editor version
        string json = File.ReadAllText(Application.dataPath + "/Prefabs/Towers/" + "TowerData.json");
        towerList = JsonUtility.FromJson<TowerList>(json);

        List<TowerContainer.Tower> towers = new List<TowerContainer.Tower>();
        foreach (TowerInfo towerInfo in towerList.Towers)
        {
            TowerContainer.Tower tower = new TowerContainer.Tower(towerInfo.name, towerInfo.attackSpeed, towerInfo.attackDamage, towerInfo.attackRange, towerInfo.cost);
            towers.Add(tower);
        }
        return towers;
    }
}
