using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Node using the tilemap grid coordinate system.
/// Used to calculate the path for each enemy type
/// </summary>
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
    /// <summary>
    /// Sets the current weight of the node(length of path to the node + movement speed coeficient)
    /// </summary>
    /// <param name="newWeight"></param>
    public void SetCurrentWeight(float newWeight)
    {
        currentWeight = newWeight;
    }
    public int CompareTo(Node other)
    {
        return currentWeight.CompareTo(other.currentWeight);
    }
}
