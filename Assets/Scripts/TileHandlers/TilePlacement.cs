using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
/// <summary>
/// Class responsible for tile and tower placement, attached to the grid on which the tiles will be placed
/// </summary>
public class TilePlacement : MonoBehaviour
{
    Tilemap tilemap;
    GameManager manager;
    PathfindingManager pathfindingManager;
    TileSelectionHandler tileSelectionHandler;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        tilemap = gameObject.GetComponentInChildren<Tilemap>();
    }
    /// <summary>
    /// Places the selected tile on mouse location(if it is not on UI).
    /// Called by the drag and click handlers
    /// </summary>
    /// <param name="position"></param>
    public void PlaceTile(Vector3 position)
    {
        //Build eventData from Input pos;

        //TODO: Can't I just pass it in????
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        if (!PointerOverUI(eventData))//Do not place if pointer is on UI
        {
            Vector3Int cellPosition = tilemap.WorldToCell(position);
            if(manager.GetSelectedTile() != null) 
            { 
                TileContainer.Tile selection = manager.GetSelectedTile();
                Tile tile = tileSelectionHandler.GetTileFromSelection(selection);
                tilemap.SetTile(cellPosition, tile);
            }

        }
    }
    

    /// <summary>
    /// Checks if pointer is hovering over a UI object
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns>True if it is</returns>
    bool PointerOverUI(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        for (int index = 0; index < raycastResults.Count; index++)
        {
            RaycastResult curRaysastResult = raycastResults[index];
            if (curRaysastResult.gameObject.layer == 5)//5 is UI value
            {
                return true;
            }
        }
        return false;
    }
}
