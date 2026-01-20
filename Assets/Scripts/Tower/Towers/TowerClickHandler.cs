using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickHandler : MonoBehaviour, IPointerClickHandler
{
    UpgradeHandler upgradeHandler;
    TileHighlighter tileHighlighter;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        upgradeHandler = gameObject.GetComponent<UpgradeHandler>();
        tileHighlighter = GameObject.Find("TileHighlighter").GetComponent<TileHighlighter>();
    }

    /// <summary>
    /// Chekcks if the tower that has been clicked on is active and opens the tower menu if it is
    /// </summary>
    /// <param name="eventData">Event data of the pointer click</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (upgradeHandler.GetShootingState())
        {
            gameManager.ActivateTowerMenu(upgradeHandler);
            tileHighlighter.HighlightTileFromPos(gameObject.transform.position);
        }
    }
}
