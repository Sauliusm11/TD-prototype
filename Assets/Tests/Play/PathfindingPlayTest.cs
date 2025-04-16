using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

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
        //Act
        parser.LoadLevelTiles("Default");
        Node expected = new Node(15, 2, 1, 1, "Portal");
        Node actual = pathfindingManager.GetNodeFromCell(new Vector3Int(6, -3));

        // Use the Assert class to test conditions.
        Assert.AreEqual(expected.GetName(), actual.GetName());
        // Use yield to skip a frame.
        yield return null;
    }
}
