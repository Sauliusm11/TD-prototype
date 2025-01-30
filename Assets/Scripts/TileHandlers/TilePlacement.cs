using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
/// <summary>
/// Class responsible for tile and tower placement, attached to the grid on which the tiles will be placed
/// (Note: probably a bad idea to have both in the same class)
/// </summary>
public class TilePlacement : MonoBehaviour
{
    Tilemap tilemap;
    GameManager manager;
    PathfindingManager pathfindingManager;
    TileSelectionHandler tileSelectionHandler;
    TowerSelectionHandler towerSelectionHandler;
    TileContainer tileContainer;
    Money moneyHandler;
    TileHighlighter tileHighlighter;
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
        tileContainer = TileContainer.getInstance();
        tileHighlighter = GameObject.Find("TileHighlighter").GetComponent<TileHighlighter>();
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
