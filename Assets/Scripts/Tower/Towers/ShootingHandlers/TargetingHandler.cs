using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers.ShootingHandlers.TargetingTypes
{
    public abstract class TargetingHandler : MonoBehaviour
    {
        public abstract List<BaseEnemy> UpdateTargets(float Range);
    }
}
