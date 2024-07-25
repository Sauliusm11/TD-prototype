using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector3 GetVector3()
    {
        return new Vector3(X, Y);
    }
    public float GetMovementSpeedCoef()
    {
        return MovementSpeedCoef;
    }
}
