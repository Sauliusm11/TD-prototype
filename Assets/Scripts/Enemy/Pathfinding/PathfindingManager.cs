using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Pathfinder manager acting as a mediator between all different pathfinding classes
/// Should only be attached to the PathFindingManager object
/// </summary>
public class PathfindingManager : MonoBehaviour
{
    WaveHandler waveHandler;
    EnemyPathFinding baseEnemyPathFinder;
    Vector3Int tilesSize;
    TileContainer tileContainer;
    Node[] Nodes;
    Node target;
    Node start;
    List<bool> PathfinderFlags = new List<bool>();
    private static object threadLock = new object();

    Tilemap tilemap;
    float xOffset;
    float yOffset;
    // Start is called before the first frame update
    void Start()
    {
        tileContainer = TileContainer.getInstance();
        baseEnemyPathFinder = GameObject.Find("BasicEnemyPathfinder").GetComponent<EnemyPathFinding>();
        PathfinderFlags.Add(false);
        waveHandler = GameObject.Find("WaveManager").GetComponent<WaveHandler>();

        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
    }
    /// <summary>
    /// Builds a 'graph' using the Node class from the tiles of the loaded level.
    /// And starts the prepare pathfinding coroutine.
    /// Should only be called on loading a new level
    /// </summary>
    /// <param name="tiles">Tile array of the level</param>
    /// <param name="size">X and Y sizes of the array(needed to keep track of 2 dimensions in a 1D array)</param>
    public void LoadLevelTileList(TileBase[] tiles,Vector3Int size)
    {
        Nodes = new Node[tiles.Length];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                TileBase tile = tiles[x + y * size.x];
                Node node = new Node(x,y,Mathf.Infinity);
                foreach (TileContainer.Tile tileInfo in tileContainer.tiles)
                {
                    if (tile.name.Equals(tileInfo.name))
                    {
                        node = new Node(x,y,tileInfo.movementSpeed);
                    }
                }
                //Reminder: this only really works with one castle and one portal
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

        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        xOffset = bounds.position.x + 0.5f;
        yOffset = bounds.position.y + 0.5f;

        tilesSize = size;
    }
    /// <summary>
    /// Adds a flag to one of the internal nodes where the tower was placed
    /// </summary>
    /// <param name="position">Cell position of the tower</param>
    public void AddTowerToNode(Vector3Int position)
    {
        //Debug.Log(position.x - Mathf.CeilToInt(xOffset) + (position.y - Mathf.CeilToInt(yOffset)) * tilesSize.x);
        Nodes[position.x - Mathf.FloorToInt(xOffset) + (position.y - Mathf.FloorToInt(yOffset)) * tilesSize.x].SetHasTower(true);
    }
    /// <summary>
    /// Removes a flag from one of the internal nodes where the tower was placed
    /// </summary>
    /// <param name="position">Cell position of the tower</param>
    public void RemoveTowerFromNode(Vector3Int position)
    {
        Nodes[position.x - Mathf.FloorToInt(xOffset) + (position.y - Mathf.FloorToInt(yOffset)) * tilesSize.x].SetHasTower(false);
    }
    /// <summary>
    /// Called by the next wave button to indicate that the player wants to start the next wave
    /// </summary>
    public void CallWave()
    {
        //TODO: add checks to prevent spamming
        StartCoroutine(PreparePathFinding());
    }
    /// <summary>
    /// Convert a Node from the grid system to the WorldNode
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public WorldNode ConvertToWorldNode(Node node)
    {
        Vector3 position = tilemap.CellToWorld(new Vector3Int(node.GetX(), node.GetY()));
        WorldNode newNode = new WorldNode(position.x + xOffset, position.y + yOffset, node.GetMovementSpeedCoef());
        return newNode;
    }
    /// <summary>
    /// Called by each individual pathfinder to indicate that it has finished the calculation.
    /// If all flags are true, calls the method responsible for the start of the wave
    /// </summary>
    /// <param name="index">Index of the flag assigned to the pathfinder</param>
    public void PathfinderFinished(int index)
    {
        lock (threadLock)
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
                waveHandler.StartWave(start,xOffset,yOffset);
            }
        }
    }
    /// <summary>
    /// Resets all readiness flags to false
    /// </summary>
    void ResetFlags()
    {
        lock (threadLock)
        {
            for (int i = 0; i < PathfinderFlags.Count; i++)
            {
                PathfinderFlags[i] = false;
            }
        } 
    }
    /// <summary>
    /// Does all the necessary preparations and signals each pathfinder to recalculate the path.
    /// </summary>
    /// <returns></returns>
    IEnumerator PreparePathFinding()
    {
        ResetFlags();
        StartCoroutine(baseEnemyPathFinder.CalculatePath(Nodes,start,target,tilesSize,0));
        yield return null;
    }
}
