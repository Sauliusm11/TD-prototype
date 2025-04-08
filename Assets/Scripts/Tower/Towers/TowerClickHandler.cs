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

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (upgradeHandler.GetShootingState()) 
        { 
            gameManager.ActivateTowerMenu(upgradeHandler);
            tileHighlighter.HighlightTileFromPos(gameObject.transform.position);
        }
    }
}
