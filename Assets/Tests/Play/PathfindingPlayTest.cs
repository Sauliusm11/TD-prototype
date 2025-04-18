using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PathfindingPlayTest
{
    bool sceneLoaded;
    bool referencesSetup;
    JsonParser JsonParser;


    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
    }

    void SetupReferences()
    {
        if (referencesSetup)
        {
            return;
        }

        Transform[] objects = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform t in objects)
        {
            if (t.name == "JsonParser")
            {
                JsonParser = t.GetComponent<JsonParser>();
            }
        }

        referencesSetup = true;
    }

    [UnityTest]
    public IEnumerator TestReferencesNotNullAfterLoad()
    {
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        Assert.IsNotNull(JsonParser);
        //Add all other references as well for quick nullref testing
        yield return null;
    }

    [UnityTest]
    public IEnumerator TileLoadingPasses()
    {
        //Wait for the scene to load
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        //Arrange
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        PathfindingManager pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        parser.LoadLevelTiles("Default");

        //Act
        Node expected = new Node(15, 2, 1, 1, "Portal");
        Node actual = pathfindingManager.GetNodeFromCell(new Vector3Int(6, -3));

        // Use the Assert class to test conditions.
        Assert.AreEqual(expected.GetName(), actual.GetName());
        // Use yield to skip a frame.
        yield return null;
    }

    static float[] testCasesExpected = { 29f };
    static float[][] TestCasesX = { new float[] { 5.5f, 5.5f, 5.5f, 6.5f, 6.5f, 8.5f } };
    static float[][] TestCasesY = { new float[] { -4.5f, -3.5f, -2.5f, -1.5f, -0.5f, -0.5f } };
    [UnityTest]
    public IEnumerator BasicPredictablePathPasses([ValueSource(nameof(testCasesExpected))] float expected, [ValueSource(nameof(TestCasesX))] float[] towerPositionsX, [ValueSource(nameof(TestCasesY))] float[] towerPositionsY)
    {
        //Wait for the scene to load
        yield return new WaitWhile(() => sceneLoaded == false);
        SetupReferences();
        //Arrange
        //Get needed managers
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        PathfindingManager pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        TowerPlacement towerPlacement = GameObject.Find("Grid").GetComponent<TowerPlacement>();
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        EnemyPathFinding enemyPathfinding = GameObject.Find("BasePathfinder").GetComponent<EnemyPathFinding>();
        //Load selected level
        parser.LoadLevelTiles("Default");

        //Possible that other test cases might need more money
        gameManager.SetSelectedTower(TowerContainer.getInstance().towers[0]);
        //Prepare mock to bypass pointer over ui check
        PointerEventData eventDataMock = new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>());
        eventDataMock.position = new Vector3(-1, -1, -1);
        //Place towers based on given test case
        for (int i = 0; i < towerPositionsX.Length; i++)
        {
            Vector3 position = new Vector3(towerPositionsX[i], towerPositionsY[i]); 
            //This needs to become a test
            towerPlacement.HandlePlaceTower(position, eventDataMock);
            towerPlacement.ConfirmPlacement();
        }

        //Act
        pathfindingManager.CallWave();
        yield return new WaitWhile(() => gameManager.IsWaveActive() == false);
        //Calculate speed of path
        Stack<WorldNode> path = enemyPathfinding.GetPath();
        float actual = 0;
        foreach (WorldNode node in path) 
        {
            actual += 1 / node.GetMovementSpeedCoef();
        }

        //Node expected = new Node(15, 2, 1, 1, "Portal");

        // Use the Assert class to test conditions.
        Assert.AreEqual(expected, actual);
        // Use yield to skip a frame.
        yield return null;
    }
}
