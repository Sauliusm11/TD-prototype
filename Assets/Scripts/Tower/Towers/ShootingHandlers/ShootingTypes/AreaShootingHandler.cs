using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers
{
    public class AreaShootingHandler : ShootingHandler
    {
        private GameObject fireIndicator;
        private GameObject rangeIndicator;

        public override void AimAtTarget()
        {
            baseEnemies = targetingHandler.UpdateTargets(range);
        }

        public override void Shoot()
        {
            baseEnemies = targetingHandler.UpdateTargets(range);
            if (baseEnemies.Count > 0)
            {
                if (fireIndicator == null)
                {
                    fireIndicator = transform.Find("FireIndicator").gameObject;
                    rangeIndicator = transform.Find("RangeIndicator").gameObject;
                    fireIndicator.transform.localScale = rangeIndicator.transform.localScale;
                    fireIndicator.transform.position = rangeIndicator.transform.position;
                }
                fireIndicator.SetActive(true);
                StartCoroutine(DisableFireIndicator(cooldown / 5));
                foreach (BaseEnemy enemy in baseEnemies)
                {
                    enemy.ReduceHealth(damage);
                }
                timeSinceShot = 0;
            }
        }

        public override GameManager.TargetingType GetTargetingType()
        {
            return GameManager.TargetingType.Area;
        }
        public override void SetTargetingType(GameManager.TargetingType newTargetingType)
        {
            //Area does not have targeting type changing
        }

        IEnumerator DisableFireIndicator(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (fireIndicator == null)
            {
                fireIndicator = transform.Find("FireIndicator").gameObject;
            }
            if (fireIndicator.activeInHierarchy)
            {
                fireIndicator.SetActive(false);
            }
        }
    }
}
