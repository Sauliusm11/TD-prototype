using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTerrainPathfinding : EnemyPathFinding
{
    protected override float CalculateWeight(Node node)
    {
        Debug.Log(node.GetDamageMultCoef());
        return 1f + Mathf.Pow((node.GetDamageMultCoef()), 2);//Adding one increases how much the enemy prioritizes saftey over speed
    }
}
