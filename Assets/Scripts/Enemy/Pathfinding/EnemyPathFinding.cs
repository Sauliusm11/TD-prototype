using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Pathfinding class of the enemies with no movement speed changes
/// </summary>
public class EnemyPathFinding : MonoBehaviour
{
    GameManager gameManager;
    PathfindingManager pathfindingManager;
    Tilemap tilemap;
    Vector3 goTo;
    Node Target;
    Stack<WorldNode> path = new Stack<WorldNode>();
    WorldNode[] pathArray;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
    }
    /// <summary>
    /// Calculate the path using A*
    /// </summary>
    /// <param name="nodes">All available nodes</param>
    /// <param name="start">Starting node</param>
    /// <param name="target">End node</param>
    /// <param name="size">X and Y sizes of the array(needed to keep track of 2 dimensions in a 1D array)</param>
    /// <param name="flagIndex">Index of this specific pathfinder flag assgined by the manager</param>
    /// <returns></returns>
    public IEnumerator CalculatePath(Node[] nodes,Node start, Node target, Vector3Int size,int flagIndex)
    {
        Target = target;
        Vector3 current = transform.position;
        PriorityQueue<Node> priorityQueue = new PriorityQueue<Node>();
        int[] cameFrom = new int[nodes.Length];
        for (int i = 0; i < cameFrom.Length; i++)
        {
            cameFrom[i] = -1;
        }
        float[] gScore = new float[nodes.Length];
        float[] fScore = new float[nodes.Length];



        int startIndex = start.GetX()+size.x*start.GetY();
        for (int i = 0; i < gScore.Length; i++) 
        {
            gScore[i] = float.PositiveInfinity;
            fScore[i] = float.PositiveInfinity;
            if(i == startIndex)
            {
                gScore[i] = 0;
                fScore[i] = CalculateDistanceToTarget(nodes[i]);
            }
            nodes[i].SetCurrentWeight(fScore[i]);
        }

        priorityQueue.Enqueue(start);
        while(priorityQueue.Count() > 0)
        {
            Node currentNode = priorityQueue.Dequeue();
            int currentIndex = currentNode.GetX()+currentNode.GetY()*size.x;
            int distanceToTarget = CalculateDistanceToTarget(currentNode);
            if(distanceToTarget <= 0)
            {
                ReconstructPath(nodes,cameFrom,currentIndex,flagIndex);
                break;
            }
            foreach (int neighbourIndex in GetNeighbourIndexes(currentIndex,size))
            {
                float tenative_gScore = gScore[currentIndex] + Mathf.Pow((1/nodes[currentIndex].GetMovementSpeedCoef()), 2);
                if (nodes[currentIndex].GetHasTower())
                {
                    tenative_gScore += 1000000;
                }
                if(tenative_gScore < gScore[neighbourIndex])
                {
                    cameFrom[neighbourIndex] = currentIndex;
                    gScore[neighbourIndex] = tenative_gScore;
                    fScore[neighbourIndex] = tenative_gScore + distanceToTarget;
                    nodes[neighbourIndex].SetCurrentWeight(fScore[neighbourIndex]);
                    if (!priorityQueue.Contains(nodes[neighbourIndex]))
                    {
                        priorityQueue.Enqueue(nodes[neighbourIndex]);
                    }
                }
            } 
            
        }
        yield return null;
    }

    /// <summary>
    /// Calculates the distance to target from given node(ignoring costs, just pure distance)
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    int CalculateDistanceToTarget(Node node)
    {
        return Mathf.Abs(node.GetX() - Target.GetX()) + Mathf.Abs(node.GetY() - Target.GetY());
    }
    /// <summary>
    /// Gets all the neighbouring node indexes into a list to iterate through
    /// </summary>
    /// <param name="index">Index of the current node</param>
    /// <param name="size">X and Y sizes of the array(needed to keep track of 2 dimensions in a 1D array)</param>
    /// <returns></returns>
    List<int> GetNeighbourIndexes(int index, Vector3Int size)
    {
        List<int> indexes = new List<int>();
        if(index - size.x >= 0)
        {
            indexes.Add(index - size.x);
            if(index % size.x != 0) 
            { 
                indexes.Add(index - 1);
            }
        }
        else
        {
            if(index - 1 >= 0)
            {
                if (index % size.x != 0)
                {
                    indexes.Add(index - 1);
                }
            }
        }
        if (index + size.x < size.x * size.y)
        {
            indexes.Add(index + size.x);
            if ((index + 1) % size.x != 0) 
            { 
                indexes.Add(index + 1);
            }
        }
        else
        {
            if (index + 1 < size.x * size.y) 
            {
                if ((index + 1) % size.x != 0)
                {
                    indexes.Add(index + 1);
                }
            }
        }
        return indexes;
    }
    /// <summary>
    /// Once the pathfinding algorithm is finished construct a path using WorldNodes and signal the manager.
    /// </summary>
    /// <param name="nodes">All available nodes</param>
    /// <param name="cameFrom">Array of indexes pointing to the previous node in the path</param>
    /// <param name="current">Current(final) node index</param>
    /// <param name="flagIndex">Index of this specific pathfinder flag assgined by the manager</param>
    void ReconstructPath(Node[] nodes, int[] cameFrom, int current,int flagIndex)
    {
        path.Clear();    
        Node currentNode = nodes[current];
        path.Push(pathfindingManager.ConvertToWorldNode(currentNode));
        while (cameFrom[current] > -1)
        {
            current = cameFrom[current];
            currentNode = nodes[current];
            path.Push(pathfindingManager.ConvertToWorldNode(currentNode));
        }
        pathfindingManager.PathfinderFinished(flagIndex);
    }
    /// <summary>
    /// Called by the enemy class to recive the path
    /// </summary>
    /// <returns>A copy of the path Stack</returns>
    public Stack<WorldNode> GetPath()
    {
        return Utility.CloneStack(path);
    }
}
