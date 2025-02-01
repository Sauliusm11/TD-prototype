using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicNoTerrainEnemy : BaseEnemy
{
    protected override ObjectPooling getEnemyPooler()
    {
        return GameObject.Find("BasicNoTerrainEnemyPooler").GetComponent<ObjectPooling>();
    }

    protected override float GetMoveSpeed(WorldNode node, float walkingSpeed)
    {
        return walkingSpeed;
    }
}
