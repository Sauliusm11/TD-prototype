using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHighlighter : MonoBehaviour
{
    Tilemap tilemap;
    PathfindingManager pathfindingManager;
    TileContainer tileContainer;

    [SerializeField]
    TMP_Text damageMultiplierText;
    [SerializeField]
    TMP_Text attackRangeText;
    [SerializeField]
    TMP_Text movementSpeedText;
    [SerializeField]
    GameObject selectedTileHighlighter;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        tileContainer = TileContainer.GetInstance();

    }

    /// <summary>
    /// Converts a world position into the tilemap cell postion and calls HighlightTileFromCellPos
    /// </summary>
    /// <param name="position">World position of the click</param>
    public void HighlightTileFromPos(Vector3 position)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        HighlightTileFromCellPos(cellPosition);
    }
    /// <summary>
    /// Places the tile highlighter on the given cell position and updates the tile information panel
    /// </summary>
    /// <param name="cellPosition">Tilemap cell position</param>
    public void HighlightTileFromCellPos(Vector3Int cellPosition)
    {

        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        //For some reason the max coordinates include one more than they should
        if (bounds.y > cellPosition.y || bounds.yMax - 1 < cellPosition.y || bounds.x > cellPosition.x || bounds.xMax - 1 < cellPosition.x)
        {
            return;
        }
        else
        {
            Node node = pathfindingManager.GetNodeFromCell(cellPosition);
            if (node != null)
            {
                selectedTileHighlighter.transform.position = cellPosition;
                string nodeName = node.GetName();
                foreach (TileContainer.Tile tile in tileContainer.tiles)
                {
                    if (tile.name.Equals(nodeName))
                    {
                        attackRangeText.text = tile.attackRange.ToString();
                        damageMultiplierText.text = tile.damageMultiplier.ToString();
                        movementSpeedText.text = tile.movementSpeed.ToString();
                        break;
                    }
                }
            }
        }
    }
}
