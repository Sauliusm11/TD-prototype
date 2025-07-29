using UnityEngine;

public class BasicEnemy : BaseEnemy
{
    protected override ObjectPooling GetEnemyPooler()
    {
        return GameObject.Find("BasicEnemyPooler").GetComponent<ObjectPooling>();
    }

    protected override float GetMoveSpeed(WorldNode node, float walkingSpeed)
    {
        return node.GetMovementSpeedCoef() * walkingSpeed;
    }
}
