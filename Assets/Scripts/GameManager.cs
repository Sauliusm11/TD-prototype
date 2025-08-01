using TMPro;
using UnityEngine;
/// <summary>
/// GameManager class, responsible for managing most UI elements and states.
/// Acts as a mediator between most class calls allowing to execute additional code before passing requests.
/// Should only be attached to the GameManager object
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum State { Saving, Loading, Playing };
    State currentState;
    [SerializeField]
    GameObject saveConfirmPanel;
    [SerializeField]
    GameObject loadConfirmPanel;
    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    GameObject towerConfirmationPanel;
    [SerializeField]
    GameObject towerMenuPanel;
    TowerMenuUpdater towerMenuUpdater;
    TMP_InputField saveFileInputField;
    TMP_InputField loadFileInputField;
    [SerializeField]
    GameObject callWaveButton;
    TMP_Text waveCountText;

    JsonParser parser;
    TowerPlacement placemetHandler;
    WaveHandler waveHandler;
    Money moneyHandler;
    Lives livesHandler;
    CleanUp objectCleaner;

    UpgradeHandler currentUpgradeHandler;
    [HideInInspector]
    public TileContainer.Tile selectedTile;
    [HideInInspector]
    public TowerContainer.Tower selectedTower;
    // Start is called before the first frame update
    void Start()
    {
        SwitchState(State.Playing);
        parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        towerMenuUpdater = towerMenuPanel.GetComponent<TowerMenuUpdater>();
        placemetHandler = GameObject.Find("Grid").GetComponent<TowerPlacement>();
        waveHandler = GameObject.Find("WaveManager").GetComponent<WaveHandler>();
        moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        livesHandler = GameObject.Find("LivesHandler").GetComponent<Lives>();
        saveFileInputField = saveConfirmPanel.GetComponentInChildren<TMP_InputField>();
        loadFileInputField = loadConfirmPanel.GetComponentInChildren<TMP_InputField>();
        objectCleaner = GameObject.Find("ObjectPoolers").GetComponent<CleanUp>();
        waveCountText = GameObject.Find("WaveCount").GetComponent<TMP_Text>();

        selectedTile = null;
        selectedTower = null;
    }
    /// <summary>
    /// Internal method to smoothly switch between states
    /// </summary>
    /// <param name="newState"></param>
    void SwitchState(State newState)
    {
        EndState();
        BeginState(newState);
    }
    /// <summary>
    /// Internal method for executing code when entering a new state
    /// </summary>
    /// <param name="newState"></param>
    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.Saving:
                saveConfirmPanel.SetActive(true);
                break;
            case State.Loading:
                loadConfirmPanel.SetActive(true);
                break;
            case State.Playing:
                break;
            default:
                break;
        }
        currentState = newState;
    }
    /// <summary>
    /// Internal method for executing code when exiting the current state
    /// </summary>
    void EndState()
    {
        switch (currentState)
        {
            case State.Saving:
                saveConfirmPanel.SetActive(false);
                break;
            case State.Loading:
                loadConfirmPanel.SetActive(false);
                break;
            case State.Playing:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Sets the game state to playing if it was saving or loading
    /// </summary>
    public void CloseFilePrompt()
    {
        if (currentState == State.Saving || currentState == State.Loading)
        {
            SwitchState(State.Playing);
        }
    }
    /// <summary>
    /// Method called when clicking the save button in the dev ui.
    /// Activates saving state(and prompt)
    /// (References do not show up, it is working)
    /// </summary>
    public void StartSaving()
    {
        if (currentState != State.Saving)
        {
            SwitchState(State.Saving);
        }
    }
    /// <summary>
    /// Method called when clicking the confirm save button in the dev ui.
    /// De-activates saving state(and prompt) and calls json parser to save the file
    /// (References do not show up, it is working)
    /// </summary>
    public void ConfirmSaving()
    {
        string filename = saveFileInputField.text + ".json";
        parser.SaveLevelTiles(filename);
        CloseFilePrompt();
    }
    /// <summary>
    /// Method called when clicking the load button in the dev ui.
    /// Activates loading state(and prompt)
    /// (References do not show up, it is working)
    /// </summary>
    public void StartLoading()
    {
        if (currentState != State.Loading)
        {
            SwitchState(State.Loading);
        }
    }
    /// <summary>
    /// Method called when clicking the confirm load button in the dev ui.
    /// De-activates loading state(and prompt) and calls json parser to load the file
    /// (References do not show up, it is working)
    /// </summary>
    public void ConfirmLoading()
    {
        string filename = loadFileInputField.text;
        ConfirmLoading(filename);
        //parser.LoadLevelTiles(filename);
        //waveHandler.LoadWaves(filename);
        //CloseFilePrompt();
    }
    //Should this not be private?
    public void ConfirmLoading(string filename)
    {
        parser.LoadLevelTiles(filename);
        waveHandler.LoadWaves(filename);
        moneyHandler.ResetMoney();
        livesHandler.ResetLives();
        objectCleaner.CleanUpObjects();
        WaveEnded();
        DeactivateGameOver();
        CloseFilePrompt();
    }
    /// <summary>
    /// Stores the selected tile internally for the TilePlacement to access 
    /// </summary>
    /// <param name="selection">The new selected tile coming from TileSelectionHandler</param>
    public void SetSelectedTile(TileContainer.Tile selection)
    {
        selectedTile = selection;
    }
    /// <summary>
    /// Returns the selected tile for the TilePlacement to access 
    /// </summary>
    /// <returns></returns>
    public TileContainer.Tile GetSelectedTile()
    {
        return selectedTile;
    }
    /// <summary>
    /// Stores the selected tile internally for the TilePlacement to access 
    /// </summary>
    /// <param name="selection">The new selected tile coming from TileSelectionHandler</param>
    public void SetSelectedTower(TowerContainer.Tower selection)
    {
        selectedTower = selection;
    }
    /// <summary>
    /// Returns the selected tile for the TilePlacement to access 
    /// </summary>
    /// <returns></returns>
    public TowerContainer.Tower GetSelectedTower()
    {
        return selectedTower;
    }
    /// <summary>
    /// Changes the position of the tower placement confirmation to be above the tower
    /// </summary>
    /// <param name="towerPos">Current position of the tower</param>
    public void MoveTowerConfirmation(Vector3 towerPos)
    {
        towerPos = new Vector3(towerPos.x, towerPos.y + 1);
        towerConfirmationPanel.transform.position = towerPos;
    }
    /// <summary>
    /// Activates the confirmation+cancelation panel and calls MoveTowerConfirmation to update the position
    /// </summary>
    /// <param name="towerPos">Position of the tower</param>
    public void ActivateTowerConfirmation(Vector3 towerPos)
    {
        towerConfirmationPanel.SetActive(true);
        MoveTowerConfirmation(towerPos);
    }
    /// <summary>
    /// Deactivates the confirmation+cancelation panel
    /// </summary>
    public void DeactivateTowerConfirmation()
    {
        towerConfirmationPanel.SetActive(false);
    }
    /// <summary>
    /// Passes the cancel request of the placement of the currently placed(but not confirmed) tower
    /// </summary
    public void CancelTowerPlacement()
    {
        placemetHandler.CancelPlacement();
    }
    /// <summary>
    /// Reactiavtes the tower menu(+range indicator)
    /// saves the current tower reference and passes it to the menu updater
    /// </summary>
    /// <param name="tower">Newly selected tower</param>
    public void ActivateTowerMenu(UpgradeHandler tower)
    {
        DeActivateTowerMenu();
        //Could become it's own function if needed in other situations?
        currentUpgradeHandler = tower;
        currentUpgradeHandler.EnableRangeIndicator();
        towerMenuPanel.SetActive(true);
        towerMenuUpdater.UpdateTowerMenu(tower);
        CancelTowerPlacement();
    }
    /// <summary>
    /// Method called when clicking the sell tower button in the tower menu.
    /// Sells the tower straight away(TODO: add a confirmation stage)
    /// (References do not show up, it is working)
    /// </summary>
    public void InitiateSellTower()
    {
        currentUpgradeHandler.SellTower();
        DeActivateTowerMenu();
    }
    /// <summary>
    /// Deactiavtes the tower menu(+range indicator)
    /// </summary>
    public void DeActivateTowerMenu()
    {
        towerMenuPanel.SetActive(false);
        if (currentUpgradeHandler != null)
        {
            currentUpgradeHandler.DisableRangeIndicator();
        }
    }
    /// <summary>
    /// Method called when clicking the regular upgrade button in the tower menu.
    /// Upgrades the tower straight away(TODO: add a confirmation stage? This one should be a togglable setting though)
    /// (References do not show up, it is working)
    /// </summary>
    public void UpgradeTower()
    {
        currentUpgradeHandler.UpgradeTower(true);
        towerMenuUpdater.UpdateTowerMenu(currentUpgradeHandler);
    }
    /// <summary>
    /// Method called when clicking the alterante elite upgrade button in the tower menu.
    /// Upgrades the tower to the alternate elite type straight away(TODO: add a confirmation stage? This one should be a togglable setting though)
    /// (References do not show up, it is working)
    /// </summary>
    public void AlternateUpgradeTower()
    {
        currentUpgradeHandler.UpgradeTower(false);
        towerMenuUpdater.UpdateTowerMenu(currentUpgradeHandler);
    }
    /// <summary>
    /// Recieve the call from the wave handler and update the wave count text and hide the next wave button
    /// </summary>
    /// <param name="wave">Number of the current wave</param>
    /// <param name="totalWaves">Number of total waves</param>
    public void WaveStarted(int wave, int totalWaves)
    {
        waveCountText.text = string.Format("Wave: {0}/{1}", wave + 1, totalWaves);
        callWaveButton.SetActive(false);
    }
    /// <summary>
    /// Recieve the call about the wave ending and reactivate the next wave button
    /// </summary>
    public void WaveEnded()
    {
        callWaveButton.SetActive(true);
    }
    public bool IsWaveActive()
    {
        return !callWaveButton.activeInHierarchy;
    }
    /// <summary>
    /// Activates the game over panel
    /// </summary>
    public void ActivateGameOver()
    {
        gameOverPanel.SetActive(true);
    }
    /// <summary>
    /// Deactivates the game over panel
    /// </summary>
    void DeactivateGameOver()
    {
        gameOverPanel.SetActive(false);
    }

}
