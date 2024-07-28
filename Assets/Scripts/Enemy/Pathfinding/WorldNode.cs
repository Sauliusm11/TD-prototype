using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A copy of the Node class, designed for the world coordinate system.
/// Used to create the path for each individual enemy
/// </summary>
public class WorldNode
{
    float X, Y;
    float MovementSpeedCoef;

    public WorldNode(float x, float y, float movementSpeedCoef)
    {
        X = x;
        Y = y;
        MovementSpeedCoef = movementSpeedCoef;
    }

    public float GetX()
    {
        return X;
    }
    public float GetY()
    {
        return Y;
    }
    /// <summary>
    /// Get the X and Y values in a Vector3
    /// </summary>
    /// <returns></returns>
    public Vector3 GetVector3()
    {
        return new Vector3(X, Y);
    }
    public float GetMovementSpeedCoef()
    {
        return MovementSpeedCoef;
    }
}
