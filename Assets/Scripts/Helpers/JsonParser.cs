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
    TileList list = new TileList();
    [System.Serializable]
    class SavedTiles
    {
        public List<int> x = new List<int>();
        public List<int> y = new List<int>();
        public List<string> Name = new List<string>();
    }


    Tilemap tilemap;
    TileSelectionHandler tileSelectionHandler;
    PathfindingManager pathfindingManager;
    //Text input fields for filenames 
    //Maybe these should be handled by gamemanager?
    TMP_InputField saveFileInputField;
    TMP_InputField loadFileInputField;
    [SerializeField]
    GameObject saveFilePrompt;
    [SerializeField]
    GameObject loadFilePrompt;
    void Start()
    {
        saveFileInputField = saveFilePrompt.GetComponentInChildren<TMP_InputField>();
        loadFileInputField = loadFilePrompt.GetComponentInChildren<TMP_InputField>();
        tileSelectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
    }
    public void SaveLevelTiles()
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
        string filename = saveFileInputField.text + ".json";
        File.WriteAllText(Application.dataPath + "/Levels/" + filename, jsonData);
    }
    public void LoadLevelTiles()
    {
        string filename = loadFileInputField.text;
        TextAsset file = Resources.Load("Levels/" + filename) as TextAsset;
        SavedTiles savedTiles = JsonUtility.FromJson<SavedTiles>(file.ToString());
        //string filename = loadFileInputField.text + ".json";
        //string jsonData = File.ReadAllText(Application.dataPath + "/Levels/" + filename);
        //SavedTiles savedTiles = JsonUtility.FromJson<SavedTiles>(jsonData);

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
        pathfindingManager.LoadLevelTileList(tilemap.GetTilesBlock(bounds), bounds.size);
    }

    public List<TileContainer.Tile> LoadTileList() 
    {
        //return File.ReadAllText(Application.dataPath + "/TileMaps/Tiles/" + "TileData.json"/*filename*/);

        //string json = File.ReadAllText(Application.dataPath + "/TileMaps/Tiles/" + "TileData.json"/*filename*/);
        //list = JsonUtility.FromJson<TileList>(json); 
        
        TextAsset file = Resources.Load("TileMaps/Tiles/" + "TileData"/*filename*/) as TextAsset;
        list = JsonUtility.FromJson<TileList>(file.ToString());

        List<TileContainer.Tile> tiles = new List<TileContainer.Tile>();
        foreach (TileInfo tileInfo in list.Tiles) 
        {
            TileContainer.Tile tile = new TileContainer.Tile(tileInfo.name,tileInfo.movementSpeed,tileInfo.damageResistance,tileInfo.attackRange);
            tiles.Add(tile);
        }
        return tiles;
    }
}
