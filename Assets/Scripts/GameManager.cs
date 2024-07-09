using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

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
        Instantiate(enemyPrefab, new Vector3(portal.X +xOffset, portal.Y + yOffset, 0), new Quaternion());//+- 0.5f to center the enemy on the tile
        Debug.Log(tilemap.CellToWorld(new Vector3Int(portal.X, portal.Y, 0)));

        TileContainer tileContainer = TileContainer.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetCastleLocation()
    {
        return new Vector3(castle.X + xOffset, castle.Y + yOffset, 0);
    }
}
