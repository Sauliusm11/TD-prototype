using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : IComparable<Node>
{
    int X, Y;
    float MovementSpeedCoef;
    float currentWeight;

    public Node(int x, int y, float movementSpeedCoef)
    {
        X = x;
        Y = y;
        MovementSpeedCoef = movementSpeedCoef;
    }

    public int GetX() 
    {
        return X;
    }
    public int GetY()
    {
        return Y;
    }
    public float GetMovementSpeedCoef() 
    {
        return MovementSpeedCoef;
    }
    public void SetCurrentWeight(float newWeight)
    {
        currentWeight = newWeight;
    }
    public int CompareTo(Node other)
    {
        return currentWeight.CompareTo(other.currentWeight);
    }
    //TileContainer tileContainer;
    //List<Tile> tiles = new List<Tile>();

    //public void BuildGraph(List<Tile> tiles)
    //{
    //    foreach (Tile tile in tiles) 
    //    {
    //        foreach (TileContainer.Tile tileInfo in tileContainer.tiles)
    //        {
    //            if (tileInfo.name.Equals(tile.name))
    //            {
    //                tiles.Add(tile);//??
    //            }
    //        }
    //    }

    //}
}
