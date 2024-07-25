using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.Tilemaps;

public class EnemyPathFinding : MonoBehaviour
{
    GameManager gameManager;
    PathfindingManager pathfindingManager;
    Tilemap tilemap;
    Vector3 goTo;
    Node Target;
    Stack<WorldNode> path = new Stack<WorldNode>();
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pathfindingManager = GameObject.Find("PathFindingManager").GetComponent<PathfindingManager>();
        tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator CalculatePath(Node[] nodes,Node start, Node target, Vector3Int size,int flagIndex)
    {
        Target = target;
        //target = gameManager.GetCastleLocation();
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
        

        //goTo = current;
        //int counter = 0;
        ////Cool, I have to implement a priority queue myself :)
        ////Actually build it in PathfindingManager
        //while (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y) > 0 && counter < 1000000)
        //{
        //    counter++;
        //    if (Utility.Equals(target.x, current.x)) 
        //    {
        //        if (target.y > current.y)
        //        {
        //            goTo.y += 1;
        //        }
        //        else
        //        {
        //            goTo.y -= 1;
        //        }
        //    }
        //    else
        //    {
        //        if (Utility.Equals(target.y, current.y)) 
        //        {
        //            if (target.x > current.x)
        //            {
        //                goTo.x += 1;
        //            }
        //            else
        //            {
        //                goTo.x -= 1;
        //            }
        //        }
        //        else
        //        {
        //            if (Mathf.Abs(target.x - current.x) > Mathf.Abs(target.y - current.y))
        //            {
        //                if (target.x > current.x)
        //                {
        //                    goTo.x += 1;
        //                }
        //                else
        //                {
        //                    goTo.x -= 1;
        //                }
        //            }
        //            else
        //            {
        //                if (target.y > current.y)
        //                {
        //                    goTo.y += 1;
        //                }
        //                else
        //                {
        //                    goTo.y -= 1;
        //                }
        //            }
        //        }
        //    }
        //    //TODO: Call moveTo and wait for it
        //    //Debug.Log(string.Format("current: {0}, goto: {1}, target: {2}", current, goTo, target));
        //    yield return StartCoroutine(MoveTo(goTo, 2));
        //    current = transform.position;
        //    goTo = current;
        //    //Debug.Log(string.Format("Current: {0}", current));
        //}
    }
    //Oh, I need to calculate it in the fake(tile) coordinate system?
    int CalculateDistanceToTarget(Node node)
    {
        return Mathf.Abs(node.GetX() - Target.GetX()) + Mathf.Abs(node.GetY() - Target.GetY());
    }
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
    void ReconstructPath(Node[] nodes, int[] cameFrom, int current,int flagIndex)
    {
        if(path.Count > 0) 
        {
            path.Clear();
        }
        Node currentNode = nodes[current];
        path.Push(pathfindingManager.ConvertToWorldNode(currentNode));
        while (cameFrom[current] > 0)
        {
            current = cameFrom[current];
            currentNode = nodes[current];
            path.Push(pathfindingManager.ConvertToWorldNode(currentNode));
        }
        pathfindingManager.PathfinderFinished(flagIndex);
    }
    //WorldNode ConvertToWorldNode(Node node)
    //{
    //    Vector3 position = tilemap.CellToWorld(new Vector3Int(node.GetX(), node.GetY()));
    //    WorldNode newNode = new WorldNode(position.x,position.y,node.GetMovementSpeedCoef()); 
    //    return newNode; 
    //}
    public Stack<WorldNode> GetPath()
    {
        return path;
    }
}
