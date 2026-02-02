using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers.ShootingHandlers.TargetingTypes
{
    internal class SquareTargeting : TargetingHandler
    {
        public override List<BaseEnemy> UpdateTargets(float Range)
        {
            List<BaseEnemy> baseEnemies = new List<BaseEnemy>();
            GameObject rangeIndicator = transform.Find("RangeIndicator").gameObject;
            Vector2 startingPos = rangeIndicator.transform.TransformPoint(rangeIndicator.transform.localPosition + new Vector3(-0.5f, -Range / 2, 0));
            Vector2 endPos = rangeIndicator.transform.TransformPoint(rangeIndicator.transform.localPosition + new Vector3(0.5f, -Range / 2 + 1, 0));
            //Change the second one to 2 for ground+air guns and both for air only guns?
            Collider2D[] colliders = Physics2D.OverlapAreaAll(startingPos, endPos, 1 << 6, 1, 1);
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
