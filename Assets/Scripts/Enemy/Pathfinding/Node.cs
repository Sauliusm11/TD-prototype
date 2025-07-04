using System;
/// <summary>
/// Node using the tilemap grid coordinate system.
/// Used to calculate the path for each enemy type
/// </summary>
public class Node : IComparable<Node>
{
    int X, Y;
    float MovementSpeedCoef;
    float DamageMultCoef;
    float currentWeight;
    bool hasTower;
    string name;

    public Node(int x, int y, float movementSpeedCoef, float damageMultCoef, string name)
    {
        X = x;
        Y = y;
        MovementSpeedCoef = movementSpeedCoef;
        DamageMultCoef = damageMultCoef;
        this.name = name;
    }

    public int GetX()
    {
        return X;
    }
    public int GetY()
    {
        return Y;
    }
    public string GetName()
    {
        return name;
    }
    public float GetMovementSpeedCoef()
    {
        return MovementSpeedCoef;
    }
    public float GetDamageMultCoef()
    {
        return DamageMultCoef;
    }
    /// <summary>
    /// Sets the current weight of the node(length of path to the node + movement speed coeficient)
    /// </summary>
    /// <param name="newWeight"></param>
    public void SetCurrentWeight(float newWeight)
    {
        currentWeight = newWeight;
    }
    public float GetCurrentWeight()
    {
        return currentWeight;
    }
    public int CompareTo(Node other)
    {
        return currentWeight.CompareTo(other.currentWeight);
    }
    public void SetHasTower(bool newState)
    {
        hasTower = newState;
    }
    public bool GetHasTower()
    {
        return hasTower;
    }
}
