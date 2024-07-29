using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelectionHandler : MonoBehaviour
{
    GameManager manager;
    [SerializeField]
    List<GameObject> TowerPrefabs;
    TowerContainer towerContainer;
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
    public void SelectTower()
    {
        if (towerContainer == null)
        {
            towerContainer = TowerContainer.getInstance();
        }
        string name = EventSystem.current.currentSelectedGameObject.name;
        name = name.Substring(6);
        foreach (TowerContainer.Tower tower in towerContainer.towers)
        {
            if (name.Equals(tower.name))
            {
                manager.SetSelectedTower(tower);
            }
        }
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
            Debug.Log(tower.name);
            if (tower.name.Equals(selection.name))
            {
                return tower;
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
