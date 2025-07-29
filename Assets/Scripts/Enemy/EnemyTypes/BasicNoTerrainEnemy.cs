using UnityEngine;

public class BasicNoTerrainEnemy : BaseEnemy
{
    protected override ObjectPooling GetEnemyPooler()
    {
        return GameObject.Find("BasicNoTerrainEnemyPooler").GetComponent<ObjectPooling>();
    }

    protected override float GetMoveSpeed(WorldNode node, float walkingSpeed)
    {
        return walkingSpeed;
    }
}
