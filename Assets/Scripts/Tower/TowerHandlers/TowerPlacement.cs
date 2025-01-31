using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TowerPlacement : MonoBehaviour
{
    Tilemap tilemap;
    GameManager manager;
    PathfindingManager pathfindingManager;
    TowerSelectionHandler towerSelectionHandler;
    TileContainer tileContainer;
    Money moneyHandler;

    GameObject currentTower;
    ObjectPooling currentPooler;
    Vector3Int currentTowerCellPosition;
    Color defaultColor;
    Color partiallyTransparenent;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        tileContainer = TileContainer.getInstance();
        GameObject towerManagerObject = GameObject.Find("TowerSelectionManager");
        if (towerManagerObject != null)
        {
            towerSelectionHandler = towerManagerObject.GetComponent<TowerSelectionHandler>();
        }
        tilemap = gameObject.GetComponentInChildren<Tilemap>();
        defaultColor = Color.white;
        partiallyTransparenent = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HandlePlaceTower(Vector3 position, PointerEventData eventData)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        manager.DeActivateTowerMenu();
        if (!PointerHelper.PointerOverTower(eventData) && PointerOverTile(cellPosition) && manager.GetSelectedTower() != null)
        {
            TowerContainer.Tower selection = manager.GetSelectedTower();
            if (moneyHandler.HasEnoughMoney(selection.cost))
            {
                GameObject tower = towerSelectionHandler.GetTowerFromSelection(selection);
                currentPooler = towerSelectionHandler.GetTowerPoolerFromBaseTower(selection);
                //Need to normalize wherever the user clicked into the middle of the cell
                position = tilemap.CellToWorld(cellPosition);
                position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z);//Adding 0.5 to place on the center of the tile

                if (tower != null && currentTower == null)//No ghost tower yet
                {
                    currentTower = currentPooler.ActivateObject(position, new Quaternion());
                    currentTowerCellPosition = cellPosition;
                    Utility.SetParentAndChildrenColors(currentTower, partiallyTransparenent);
                    manager.ActivateTowerConfirmation(position);
                }
                else //Move existing placement
                {
                    if (currentTower != null)
                    {
                        currentTowerCellPosition = cellPosition;
                        currentTower.transform.position = position;
                        manager.MoveTowerConfirmation(position);
                    }
                }
                if(tower != null)
                {
                    //Figure out best way to get tile name and to reset->apply buff
                    Node node = pathfindingManager.GetNodeFromCell(cellPosition);
                    string name = node.GetName();
                    UpgradeHandler handler = currentTower.GetComponent<UpgradeHandler>();
                    foreach (TileContainer.Tile tile in tileContainer.tiles)
                    {
                        if (tile.name.Equals(name))
                        {
                            handler.ResetBuffs();
                            handler.ApplyBuff(tile);
                            break;
                        }
                    }
                }
            }
        }
    }

    //TODO: what to do with this
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

    /// <summary>
    /// Finalize placement of the tower
    /// </summary>
    public void ConfirmPlacement()
    {
        TowerContainer.Tower selection = manager.GetSelectedTower();
        if (currentTower != null && moneyHandler.RemoveMoney(selection.cost))
        {
            //Range indicator needs to stay transparent
            List<string> exclude = new List<string>
            {
                "RangeIndicator"
            };
            Utility.SetParentAndChildrenColors(currentTower, defaultColor, exclude);
            Node node = pathfindingManager.AddTowerToNode(currentTowerCellPosition);
            string name = node.GetName();
            UpgradeHandler handler = currentTower.GetComponent<UpgradeHandler>();
            foreach (TileContainer.Tile tile in tileContainer.tiles)
            {
                if (tile.name.Equals(name))
                {
                    handler.ResetBuffs();
                    handler.ApplyBuff(tile);
                    break;
                }
            }
            handler.DisableRangeIndicator();
            manager.DeactivateTowerConfirmation();
            handler.EnableTower();
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
            currentPooler.DeactivateObject(currentTower);
            currentTower = null;
            manager.DeactivateTowerConfirmation();
            //These kind of make the cancel button make sense (deselecting)
            manager.SetSelectedTower(null);
            towerSelectionHandler.DeactivateSelectionHighlighter();
        }
    }
}
