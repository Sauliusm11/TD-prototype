using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// GameManager class, responsible for managing most UI elements and states.
/// Acts as a bridge between most class calls allowing to execute additional code before passing requests. 
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum State { Saving, Loading, Playing};
    State currentState;
    [SerializeField]
    GameObject saveConfirmPanel;
    [SerializeField]
    GameObject loadConfirmPanel;
    [SerializeField]
    GameObject towerConfirmationPanel;
    TMP_InputField saveFileInputField;
    TMP_InputField loadFileInputField;
    JsonParser parser;

    [HideInInspector]
    public TileContainer.Tile selectedTile;
    [HideInInspector]
    public TowerContainer.Tower selectedTower;
    // Start is called before the first frame update
    void Start()
    {
        SwitchState(State.Playing);
        parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        saveFileInputField = saveConfirmPanel.GetComponentInChildren<TMP_InputField>();
        loadFileInputField = loadConfirmPanel.GetComponentInChildren<TMP_InputField>();

        TileContainer tileContainer = TileContainer.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if(currentState != State.Saving)
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
        parser.LoadLevelTiles(filename);
        CloseFilePrompt();
    }
    public void ConfirmLoading(string filename)
    {
        parser.LoadLevelTiles(filename);
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
    public void MoveTowerConfirmation(Vector3 towerPos)
    {
        towerPos = new Vector3(towerPos.x, towerPos.y + 1);
        towerConfirmationPanel.transform.position = towerPos;
    }
    public void ActivateTowerConfirmation(Vector3 towerPos)
    {
        towerConfirmationPanel.SetActive(true);
        MoveTowerConfirmation(towerPos);
    }
    public void DeactivateTowerConfirmation()
    {
        towerConfirmationPanel.SetActive(false);
    }
}
