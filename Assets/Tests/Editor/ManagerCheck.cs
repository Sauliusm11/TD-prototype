using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

public class ManagerCheck : MonoBehaviour
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator GameManagerFoundPasses()
    {
        //Arrange
        GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Use the Assert class to test conditions.
        Assert.IsNotNull(manager);
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator JsonParserFoundPasses()
    {
        //Arrange
        JsonParser parser = GameObject.Find("JsonParser").GetComponent<JsonParser>();
        // Use the Assert class to test conditions.
        Assert.IsNotNull(parser);
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator PathfindingManagerFoundPasses()
    {
        //Arrange
        PathfindingManager pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        // Use the Assert class to test conditions.
        Assert.IsNotNull(pathfindingManager);
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator WaveManagerFoundPasses()
    {
        //Arrange
        WaveHandler manager = GameObject.Find("WaveManager").GetComponent<WaveHandler>();
        // Use the Assert class to test conditions.
        Assert.IsNotNull(manager);
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator BasePathfinderFoundPasses()
    {
        //Arrange
        BasicPathfinding pathfinder = GameObject.Find("BasePathfinder").GetComponent<BasicPathfinding>();
        // Use the Assert class to test conditions.
        Assert.IsNotNull(pathfinder);
        // Use yield to skip a frame.
        yield return null;
    }
    [UnityTest]
    public IEnumerator NoTerrainPathfinderFoundPasses()
    {
        //Arrange
        NoTerrainPathfinding pathfinder = GameObject.Find("NoTerrainPathfinder").GetComponent<NoTerrainPathfinding>();
        // Use the Assert class to test conditions. 
        Assert.IsNotNull(pathfinder);
        // Use yield to skip a frame.
        yield return null;
    }
}

