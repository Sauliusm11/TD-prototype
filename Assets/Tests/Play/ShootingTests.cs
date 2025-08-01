using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ShootingTest
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

    static float[][] TestCasesX = { new float[] { 5.5f, 5.5f, 5.5f, 6.5f, 6.5f, 8.5f } };
    static float[][] TestCasesY = { new float[] { -4.5f, -3.5f, -2.5f, -1.5f, -0.5f, -0.5f } };
    [UnityTest]
    public IEnumerator BasicProjectileShootingPasses([ValueSource(nameof(TestCasesX))] float[] towerPositionsX, [ValueSource(nameof(TestCasesY))] float[] towerPositionsY)
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
        Money moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        Lives livesHandler = GameObject.Find("LivesHandler").GetComponent<Lives>();
        //Load selected level
        gameManager.ConfirmLoading("Default");

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
            //Skip a frame to allow start to fire first
            yield return null;
            towerPlacement.ConfirmPlacement();
        }

        //Act
        pathfindingManager.CallWave();
        while (moneyHandler.HasEnoughMoney(1))
        {
            moneyHandler.RemoveMoney(1);
        }
        yield return new WaitWhile(() => gameManager.IsWaveActive() == false);
        yield return new WaitWhile(() => moneyHandler.HasEnoughMoney(1) == false && livesHandler.HasEnoughLives(20));
        Assert.IsTrue(moneyHandler.HasEnoughMoney(1));
    }

    [UnityTest]
    public IEnumerator FastProjectileShootingPasses([ValueSource(nameof(TestCasesX))] float[] towerPositionsX, [ValueSource(nameof(TestCasesY))] float[] towerPositionsY)
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
        Money moneyHandler = GameObject.Find("MoneyHandler").GetComponent<Money>();
        Lives livesHandler = GameObject.Find("LivesHandler").GetComponent<Lives>();
        //Load selected level
        gameManager.ConfirmLoading("Default");

        //Possible that other test cases might need more money
        gameManager.SetSelectedTower(TowerContainer.getInstance().towers[1]);
        //Prepare mock to bypass pointer over ui check
        PointerEventData eventDataMock = new PointerEventData(GameObject.Find("EventSystem").GetComponent<EventSystem>());
        eventDataMock.position = new Vector3(-1, -1, -1);
        //Place towers based on given test case
        for (int i = 0; i < towerPositionsX.Length; i++)
        {
            moneyHandler.ResetMoney();
            Vector3 position = new Vector3(towerPositionsX[i], towerPositionsY[i]);
            towerPlacement.HandlePlaceTower(position, eventDataMock);
            //Skip a frame to allow start to fire first
            yield return null;
            towerPlacement.ConfirmPlacement();
        }

        //Act
        pathfindingManager.CallWave();
        while (moneyHandler.HasEnoughMoney(1))
        {
            moneyHandler.RemoveMoney(1);
        }
        yield return new WaitWhile(() => gameManager.IsWaveActive() == false);
        yield return new WaitWhile(() => moneyHandler.HasEnoughMoney(1) == false && livesHandler.HasEnoughLives(20));
        Assert.IsTrue(GameObject.FindGameObjectsWithTag("Bullet").Length < 6);
    }
}
