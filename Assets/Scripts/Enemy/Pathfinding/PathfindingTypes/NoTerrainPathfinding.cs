using UnityEngine;

public class NoTerrainPathfinding : EnemyPathFinding
{
    protected override float CalculateWeight(Node node)
    {
        return 1f + Mathf.Pow((node.GetDamageMultCoef()), 2);//Adding one increases how much the enemy prioritizes saftey over speed
    }
}
