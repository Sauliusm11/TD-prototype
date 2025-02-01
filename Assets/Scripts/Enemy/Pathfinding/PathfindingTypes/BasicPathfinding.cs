using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPathfinding : EnemyPathFinding
{
    protected override float CalculateWeight(Node node)
    {
        return Mathf.Pow((1 / node.GetMovementSpeedCoef()), 2);
    }
}
