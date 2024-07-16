using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public enum State { Saving, Loading, Playing};
    State currentState;
    [SerializeField]
    GameObject saveConfirmPanel;
    [SerializeField]
    GameObject loadConfirmPanel;
    JsonParser parser;

    [HideInInspector]
    public TileContainer.Tile selectedTile;
    Tilemap tilemap;
    class TileInfo
    {
        public int X;
        public int Y;
        public TileBase Tile;

        public TileInfo(int x, int y, TileBase tile)
        {
            X = x;
            Y = y;
            Tile = tile;
        }
    }
    TileInfo portal;
    TileInfo castle;
    float xOffset;
    float yOffset;

    [SerializeField]
    GameObject enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        SwitchState(State.Playing);
        parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    if (tile.name.Contains("Portal"))
                    {
                        portal = new TileInfo(x,y,tile);
                    }
                    if (tile.name.Contains("Castle"))
                    {
                        castle = new TileInfo(x, y, tile);
                    }

                }
            }
        }
        Debug.Log(string.Format("{0}, {1}, {2}",portal.Tile, portal.X, portal.Y));
        Debug.Log(string.Format("{0}, {1}, {2}", castle.Tile, castle.X, castle.Y));
        xOffset = bounds.position.x + 0.5f;
        yOffset = bounds.position.y + 0.5f;
        Instantiate(enemyPrefab, new Vector3(portal.X + xOffset, portal.Y + yOffset, 0), new Quaternion());//+- 0.5f to center the enemy on the tile
        Debug.Log(tilemap.CellToWorld(new Vector3Int(portal.X, portal.Y, 0)));

        TileContainer tileContainer = TileContainer.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SwitchState(State newState)
    {
        EndState();
        BeginState(newState);
    }
    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.Saving:
                saveConfirmPanel.SetActive(true);
                break;
            case State.Loading:
                loadConfirmPanel.SetActive(true);
                break;
            case State.Playing:
                break;
            default:
                break;
        }
        currentState = newState;
    }
    void EndState()
    {
        switch (currentState)
        {
            case State.Saving:
                saveConfirmPanel.SetActive(false);
                break;
            case State.Loading:
                loadConfirmPanel.SetActive(false);
                break;
            case State.Playing:
                break;
            default:
                break;
        }
    }
    public void CloseFilePrompt()
    {
        if (currentState == State.Saving || currentState == State.Loading)
        {
            SwitchState(State.Playing);
        }
    }
    public void StartSaving()
    {
        if(currentState != State.Saving)
        {
            SwitchState(State.Saving);
        }
    }
    public void ConfirmSaving()
    {
        parser.SaveLevelTiles();
        CloseFilePrompt();
    }
    public void StartLoading()
    {
        if (currentState != State.Loading)
        {
            SwitchState(State.Loading);
        }
    }
    public void ConfirmLoading()
    {
        parser.LoadLevelTiles();
        CloseFilePrompt();
    }
    public void SetSelectedTile(TileContainer.Tile selection)
    {
        selectedTile = selection;
    }
    public TileContainer.Tile GetSelectedTile()
    {
        return selectedTile;
    }
    public Vector3 GetCastleLocation()
    {
        return new Vector3(castle.X + xOffset, castle.Y + yOffset, 0);
    }
}
