using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
/// <summary>
/// Class responsible for tile placement, attached to the grid on which the tiles will be placed
/// </summary>
public class TilePlacement : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    Tilemap tilemap;
    GameManager manager;
    TileSelectionHandler selectionHandler;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        selectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
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
            TileContainer.Tile selection = manager.GetSelectedTile();
            Tile tile = selectionHandler.GetTileFromSelection(selection);
            tilemap.SetTile(cellPosition, tile);
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
