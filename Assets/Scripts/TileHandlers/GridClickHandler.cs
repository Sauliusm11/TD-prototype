using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridClickHandler : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    TilePlacement tilePlacementHandler;
    TowerPlacement towerPlacementHandler;
    TileHighlighter tileHighlighter;
    bool devMode;
  
    // Start is called before the first frame update
    void Start()
    {
        GameObject gridGameObject = GameObject.Find("Grid");
        tilePlacementHandler = gridGameObject.GetComponent<TilePlacement>();
        towerPlacementHandler = gridGameObject.GetComponent<TowerPlacement>();
        devMode = true;
        tileHighlighter = GameObject.Find("TileHighlighter").GetComponent<TileHighlighter>();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        if (towerManagerObject != null)
        {
            devMode = false;
        }

    }
    /// <summary>
    /// Called when the grid is clicked (references do not show up, it is working)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        HandlePlacement(eventData);
    }
    /// <summary>
    /// Called when mouse is being held and dragged on the grid allowing for painting (references do not show up, it is working)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        HandlePlacement(eventData);
    }
    /// <summary>
    /// Places the selected tile on mouse location(if it is not on UI).
    /// Called by the drag and click handlers
    /// </summary>
    /// <param name="eventData"></param>
    void HandlePlacement(PointerEventData eventData)
    {
        Vector3 position = eventData.pointerCurrentRaycast.worldPosition;
        if (!PointerHelper.PointerOverUI(eventData))//Do not place if pointer is on UI
        {
            if (devMode)//Placing tiles
            {
                tilePlacementHandler.PlaceTile(position);

                tileHighlighter.HighlightTileFromPos(position);
            }
            else //Placing towers
            {
                towerPlacementHandler.HandlePlaceTower(position,eventData);

                tileHighlighter.HighlightTileFromPos(position);
            }
        }
    }
}
