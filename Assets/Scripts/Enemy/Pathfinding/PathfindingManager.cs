using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static JsonParser;

public class PathfindingManager : MonoBehaviour
{
    EnemyPathFinding baseEnemyPathFinder;
    TileBase[] Tiles;
    Vector3Int tilesSize;
    TileContainer tileContainer;
    Node[] Nodes;
    Node target;
    Node start;
    class TileInfoInternal
    {
        public int X;
        public int Y;
        public TileBase Tile;

        public TileInfoInternal(int x, int y, TileBase tile)
        {
            X = x;
            Y = y;
            Tile = tile;
        }
    }
    Tilemap tilemap;
    List<bool> PathfinderFlags = new List<bool>();

    GameManager gameManager;
    //Should get these from gamemanager
    TileInfoInternal portal;
    TileInfoInternal castle;
    float xOffset;
    float yOffset;
    [SerializeField]
    GameObject enemyPrefab;
    // Start is called before the first frame update
    void Start()
    {
        tileContainer = TileContainer.getInstance();
        baseEnemyPathFinder = GameObject.Find("BasicEnemyPathfinder").GetComponent<EnemyPathFinding>();
        PathfinderFlags.Add(false);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

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
                        portal = new TileInfoInternal(x, y, tile);
                    }
                    if (tile.name.Contains("Castle"))
                    {
                        castle = new TileInfoInternal(x, y, tile);
                    }

                }
            }
        }
        xOffset = bounds.position.x + 0.5f;
        yOffset = bounds.position.y + 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadLevelTileList(TileBase[] tiles,Vector3Int size)
    {
        //Do I even need to store tiles?
        Tiles = tiles;
        Nodes = new Node[tiles.Length];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                TileBase tile = Tiles[x + y * size.x];
                Node node = new Node(x,y,Mathf.Infinity);
                foreach (TileContainer.Tile tileInfo in tileContainer.tiles)
                {
                    if (tile.name.Equals(tileInfo.name))
                    {
                        node = new Node(x,y,tileInfo.movementSpeed);
                    }

                }
                if (tile.name.Contains("Portal"))
                {
                    start = new Node(x, y, 1);
                }
                if (tile.name.Contains("Castle"))
                {
                    target = new Node(x, y, 1);
                }
                Nodes[x + y * size.x] = node;
            }
        }
        tilesSize = size;
        StartCoroutine(PreparePathFinding());
        //
    }

    public WorldNode ConvertToWorldNode(Node node)
    {
        Vector3 position = tilemap.CellToWorld(new Vector3Int(node.GetX(), node.GetY()));
        WorldNode newNode = new WorldNode(position.x+xOffset, position.y+yOffset, node.GetMovementSpeedCoef());
        return newNode;
    }
    public void PathfinderFinished(int index)
    {
        PathfinderFlags[index] = true;
        bool goodToGo = true;
        for (int i = 0; i < PathfinderFlags.Count; i++)
        {
            if (!PathfinderFlags[i])
            {
                goodToGo = false;
                break;
            }
        }
        if (goodToGo)
        { 
            //Place wave starting call here
            Instantiate(enemyPrefab, new Vector3(portal.X + xOffset, portal.Y + yOffset, 0), new Quaternion());//+- 0.5f to center the enemy on the tile
        }
    }
    void ResetFlags()
    {
        for (int i = 0; i < PathfinderFlags.Count; i++)
        {
            PathfinderFlags[i] = false;
        } 
    }
    IEnumerator PreparePathFinding()
    {
        //Need to work out how to do the wait for all thing once there are more pathfinders
        StartCoroutine(baseEnemyPathFinder.CalculatePath(Nodes,start,target,tilesSize,0));
        yield return null;
    }
}
