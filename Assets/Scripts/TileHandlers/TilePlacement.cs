using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
/// <summary>
/// Class responsible for tile and tower placement, attached to the grid on which the tiles will be placed
/// (Note: probably a bad idea to have both in the same class)
/// </summary>
public class TilePlacement : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    Tilemap tilemap;
    GameManager manager;
    PathfindingManager pathfindingManager;
    TileSelectionHandler tileSelectionHandler;
    TowerSelectionHandler towerSelectionHandler;
    Money moneyHandler;
    bool devMode;


    GameObject currentTower;
    ObjectPooling currentPooler;
    Vector3Int currentTowerCellPosition;
    Vector3 currentTowerPosition;
    Color defaultColor;
    Color partiallyTransparenent;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        devMode = true;
        tileSelectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        if (towerManagerObject != null) 
        {
            devMode = false;
            towerSelectionHandler = towerManagerObject.GetComponent<TowerSelectionHandler>();
        }
        tilemap = gameObject.GetComponentInChildren<Tilemap>();
        defaultColor = Color.white;
        partiallyTransparenent = new Color(defaultColor.r, defaultColor.g, defaultColor.b,0.5f);
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
            if (devMode)//Placing tiles
            {
                if(manager.GetSelectedTile() != null) 
                { 
                    TileContainer.Tile selection = manager.GetSelectedTile();
                    Tile tile = tileSelectionHandler.GetTileFromSelection(selection);
                    tilemap.SetTile(cellPosition, tile);
                }
            }
            else //Placing towers
            {
                if (!PointerOverTower(eventData) && PointerOverTile(cellPosition) && manager.GetSelectedTower() != null)
                {
                    TowerContainer.Tower selection = manager.GetSelectedTower();
                    if (moneyHandler.HasEnoughMoney(selection.cost))
                    {
                        GameObject tower = towerSelectionHandler.GetTowerFromSelection(selection);
                        currentPooler = towerSelectionHandler.GetTowerPoolerFromSelection(selection);
                        position = tilemap.CellToWorld(cellPosition);
                        position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z);//Adding 0.5 to place on the center of the tile
                        
                        if (tower != null && currentTower == null)//No ghost tower yet
                        {
                            //currentTower = Instantiate(tower, position, new Quaternion());
                            currentTower = currentPooler.ActivateObject(position, new Quaternion());
                            currentTowerPosition = position;
                            currentTowerCellPosition = cellPosition;
                            Utility.SetParentAndChildrenColors(currentTower,partiallyTransparenent);
                            manager.ActivateTowerConfirmation(position);
                        }
                        else //Move existing placement
                        {
                            if(currentTower != null) 
                            { 
                                currentTowerPosition = position;
                                currentTowerCellPosition = cellPosition;
                                currentTower.transform.position = position;
                                position = tilemap.CellToWorld(cellPosition);
                                position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z);//Adding 0.5 to place on the center of the tile
                                manager.MoveTowerConfirmation(position);
                            }
                        }
                        //TODO: consider adding object pooling
                    }
                }
            }
        }
    }
    /// <summary>
    /// Finalize placement of the tower
    /// </summary>
    public void ConfirmPlacement()
    {
        TowerContainer.Tower selection = manager.GetSelectedTower();
        if (currentTower != null && moneyHandler.RemoveMoney(selection.cost))
        {
            Utility.SetParentAndChildrenColors(currentTower, defaultColor);
            pathfindingManager.AddTowerToNode(currentTowerCellPosition);
            manager.DeactivateTowerConfirmation();
            currentTower.GetComponent<ShootingHandler>().EnableTower();
            currentTower = null;

            //Uncomment these to deselect after buying a tower(should be a setting in the future)
            //manager.SetSelectedTower(null);
            //towerSelectionHandler.DeactivateSelectionHighlighter();
        }
    }
    /// <summary>
    /// Cancels the placement of the currently placed(but not confirmed) tower
    /// </summary>
    public void CancelPlacement()
    {
        if (currentTower != null)
        {
            //Destroy(currentTower);//Again, object pooling ftw, but this is easier
            currentPooler.DeactivateObject(currentTower);
            currentTower = null;
            manager.DeactivateTowerConfirmation();
            //These kind of make the cancel button make sense (deselecting)
            manager.SetSelectedTower(null);
            towerSelectionHandler.DeactivateSelectionHighlighter();
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
    /// <summary>
    /// Checks if pointer is hovering over a Tower object
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns>True if it is</returns>
    bool PointerOverTower(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        for (int index = 0; index < raycastResults.Count; index++)
        {
            RaycastResult curRaysastResult = raycastResults[index];
            Debug.Log(curRaysastResult.gameObject.name);
            if (curRaysastResult.gameObject.layer == 3)//3 is Tower value
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if pointer is hovering over a valid tile
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <returns>True if it is</returns>
    bool PointerOverTile(Vector3Int cellPosition)
    {
        if (tilemap.GetTile(cellPosition) != null)
        {

            return true; 
        }

        return false;
    }

}
