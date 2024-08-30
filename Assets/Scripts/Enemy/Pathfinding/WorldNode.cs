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
    string name;
    public WorldNode(float x, float y, float movementSpeedCoef, string name)
    {
        X = x;
        Y = y;
        MovementSpeedCoef = movementSpeedCoef;
        this.name = name;
    }

    public float GetX()
    {
        return X;
    }
    public float GetY()
    {
        return Y;
    }
    public string GetName()
    {
        return name;
    }
    /// <summary>
    /// Get the X and Y values in a Vector3
    /// </summary>
    /// <returns></returns>
    public Vector3 GetVector3()
    {
        return new Vector3(X, Y, 1);//All enemies should be at z=1(could do 2 for flying?)
    }
    public float GetMovementSpeedCoef()
    {
        return MovementSpeedCoef;
    }
}
