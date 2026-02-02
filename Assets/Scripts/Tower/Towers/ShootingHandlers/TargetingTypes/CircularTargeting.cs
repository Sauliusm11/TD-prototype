using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers.ShootingHandlers.TargetingTypes
{
    internal class CircularTargeting : TargetingHandler
    {
        public override List<BaseEnemy> UpdateTargets(float Range)
        {
            List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
            //Change the second one to 2 for ground+air guns and both for air only guns?
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, Range, 1 << 6, 1, 1);
            //There has to be a better way, right?
            baseEnemies.Clear();
            foreach (Collider2D collider in colliders)
            {
                GameObject collisionGameObject = collider.gameObject;
                if (collisionGameObject.TryGetComponent<BaseEnemy>(out var baseEnemy))
                {
                    baseEnemies.Add(baseEnemy);
                }
            }
            return baseEnemies;
        }
    }
}
