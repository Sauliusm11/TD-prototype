using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
/// <summary>
/// Class responsible for selecting tiles to place in the level editor
/// </summary>
public class TileSelectionHandler : MonoBehaviour
{
    GameManager manager;
    [SerializeField]
    List<Tile> Tiles;
    TileContainer tileContainer;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    /// <summary>
    /// Method called by each of the tile selection buttons.
    /// Sets the currently selected stored in the GameManager.
    /// (References do not show up, it is working)
    /// </summary>
    public void SelectSquare()
    {
        if (tileContainer == null)
        {
            tileContainer = TileContainer.getInstance();
        }
        string name = EventSystem.current.currentSelectedGameObject.name;
        name = name.Substring(6);
        foreach (TileContainer.Tile tile in tileContainer.tiles)
        {
            if (name.Equals(tile.name))
            {
                TileContainer.Tile selectedTile = manager.GetSelectedTile();
                if (selectedTile != null && selectedTile.name.Equals(tile.name))
                {
                    manager.SetSelectedTile(null);
                }
                else
                {
                    manager.SetSelectedTile(tile);
                }
            }
        }
    }
    /// <summary>
    /// Method called when placing the tile, should only be called by TilePlacement
    /// </summary>
    /// <param name="selection">The selected tile type</param>
    /// <returns>Actual tile object to place in the grid</returns>
    public Tile GetTileFromSelection(TileContainer.Tile selection)
    {
        foreach (Tile tile in Tiles)
        {
            if (tile.name.Equals(selection.name))
            {
                return tile;
            }
        }
        //Removal
        return null;
    }
    /// <summary>
    /// Called by methods loading the level (should be only JsonParser)
    /// </summary>
    /// <returns>List of all unique tile objects</returns>
    public List<Tile> GetTileList()
    {
        return Tiles;
    }
}
