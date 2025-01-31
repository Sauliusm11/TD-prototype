using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelectionHandler : MonoBehaviour
{
    GameManager manager;
    [SerializeField]
    List<GameObject> TowerPrefabs;
    [SerializeField]
    List<ObjectPooling> ObjectPoolers;
    TowerContainer towerContainer;
    TilePlacement placemetHandler;
    [SerializeField]
    GameObject selectionHighlighter;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        placemetHandler = GameObject.Find("Grid").GetComponent<TilePlacement>();
    }

    /// <summary>
    /// Method called by each of the tile selection buttons.
    /// Sets the currently selected stored in the GameManager.
    /// (References do not show up, it is working)
    /// </summary>
    public void SelectTower()
    {
        if (towerContainer == null)
        {
            towerContainer = TowerContainer.getInstance();
        }
        GameObject buttonObject = EventSystem.current.currentSelectedGameObject;
        string name = buttonObject.name;
        name = name.Substring(6);
        manager.CancelTowerPlacement();
        foreach (TowerContainer.Tower tower in towerContainer.towers)
        {
            if (name.Equals(tower.name))
            {
                TowerContainer.Tower selectedTower = manager.GetSelectedTower();
                if (selectedTower != null && selectedTower.name.Equals(tower.name)) 
                {
                    manager.SetSelectedTower(null);
                    DeactivateSelectionHighlighter();
                }
                else 
                { 
                    manager.SetSelectedTower(tower);
                    selectionHighlighter.SetActive(true);
                    selectionHighlighter.transform.position = buttonObject.transform.position;
                }
            }
        }
    }
    /// <summary>
    /// Deactivates the selection highlighter panel
    /// </summary>
    public void DeactivateSelectionHighlighter()
    {
        selectionHighlighter.SetActive(false);
    }
    /// <summary>
    /// Method called when placing the tile, should only be called by TilePlacement
    /// </summary>
    /// <param name="selection">The selected tile type</param>
    /// <returns>Actual tile object to place in the grid</returns>
    public GameObject GetTowerFromSelection(TowerContainer.Tower selection)
    {
        foreach (GameObject tower in TowerPrefabs)
        {
            if (tower.name.Equals(selection.name))
            {
                return tower;
            }
        }
        //Removal, should never be called
        return null;
    }
    /// <summary>
    /// Gets the object pooler responsible for the currently selected tower type
    /// </summary>
    /// <param name="selection">Currently selected tower type</param>
    /// <returns>Object pooling object</returns>
    public ObjectPooling GetTowerPoolerFromBaseTower(TowerContainer.Tower selection)
    {
        foreach (ObjectPooling pooler in ObjectPoolers)
        {
            if (pooler.name.Contains(selection.name))
            {
                return pooler;
            }
        }
        //Removal, should never be called
        return null;
    }
    /// <summary>
    /// Called by methods loading the level (should be only JsonParser)
    /// </summary>
    /// <returns>List of all unique tower objects</returns>
    public List<GameObject> GetTileList()
    {
        return TowerPrefabs;
    }
}
