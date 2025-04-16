using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

    public class PathFindingTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {

        // Use the Assert class to test conditions


        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TileLoadingPasses()
        {
        //Arrange
        Tilemap tilemap;
        TileSelectionHandler tileSelectionHandler;
        tileSelectionHandler = GameObject.Find("TileSelectionManager").GetComponent<TileSelectionHandler>();
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        PathfindingManager pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        //Act
        parser.LoadLevelTiles("Default");
        Node expected = new Node(15, 2, 1, 1, "Portal");
        Node actual = pathfindingManager.GetNodeFromCell(new Vector3Int(15, 2));

        // Use the Assert class to test conditions.
        Assert.AreEqual(expected, actual);
        // Use yield to skip a frame.
        yield return null;
        }
    }

