using UnityEngine;

public class NoTerrainPathfinding : EnemyPathFinding
{
    const float speedOverSafetyCoef = 1f; //Bigger number = bigger emphasis on shorter walking distance

    protected override float CalculateWeight(Node node)
    {
        return speedOverSafetyCoef + Mathf.Pow((node.GetDamageMultCoef()), 2);
    }
}
