using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
/// <summary>
/// Class responsible for tile placement, attached to the grid on which the tiles will be placed
/// </summary>
public class TilePlacement : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    Tilemap tilemap;
    GameManager manager;
    PathfindingManager pathfindingManager;
    TileSelectionHandler tileSelectionHandler;
    TowerSelectionHandler towerSelectionHandler;
    bool devMode;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        devMode = false;
        tileSelectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        if (towerManagerObject != null) 
        {
            devMode = false;
            towerSelectionHandler = towerManagerObject.GetComponent<TowerSelectionHandler>();
        }
        tilemap = gameObject.GetComponentInChildren<Tilemap>();
    }
    /// <summary>
    /// Called when the grid is clicked (references do not show up, it is working)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaceTile(eventData.pointerCurrentRaycast.worldPosition);
    }
    /// <summary>
    /// Called when mouse is being held and dragged on the grid allowing for painting (references do not show up, it is working)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        PlaceTile(eventData.pointerCurrentRaycast.worldPosition);
    }
    /// <summary>
    /// Places the selected tile on mouse location(if it is not on UI).
    /// Called by the drag and click handlers
    /// </summary>
    /// <param name="position"></param>
    void PlaceTile(Vector3 position)
    {
        //Build eventData from Input pos;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        if (!PointerOverUI(eventData))//Do not place if pointer is on UI
        {
            Vector3Int cellPosition = tilemap.WorldToCell(position);
            if (devMode)
            {
                TileContainer.Tile selection = manager.GetSelectedTile();
                Tile tile = tileSelectionHandler.GetTileFromSelection(selection);
                tilemap.SetTile(cellPosition, tile);
            }
            else 
            {
                TowerContainer.Tower selection = manager.GetSelectedTower();
                GameObject tower = towerSelectionHandler.GetTowerFromSelection(selection);
                position = tilemap.CellToWorld(cellPosition);
                position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z);//Adding 0.5 to place on the center of the tile
                if (tower != null)
                {
                    Instantiate(tower, position, new Quaternion());
                    pathfindingManager.AddTowerToNode(cellPosition);
                }
                //TODO: consider adding object pooling
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
