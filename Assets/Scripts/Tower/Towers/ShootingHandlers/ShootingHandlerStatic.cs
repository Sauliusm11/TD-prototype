using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers
{
    public class ShootingHandlerStatic : ShootingHandler
    {
        private GameObject fireIndicator;
        private GameObject rangeIndicator;

        public override void AimAtTarget()
        {
            UpdateTargets();
        }

        public override void Shoot()
        {
            UpdateTargets();
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

        public override void UpdateTargets()
        {
            GameObject rangeIndicator = transform.Find("RangeIndicator").gameObject;
            Vector2 startingPos = rangeIndicator.transform.TransformPoint(rangeIndicator.transform.localPosition + new Vector3(-0.5f, -range / 2, 0));
            Vector2 endPos = rangeIndicator.transform.TransformPoint(rangeIndicator.transform.localPosition + new Vector3(0.5f, -range / 2 + 1, 0));
            //Change the second one to 2 for ground+air guns and both for air only guns?
            Collider2D[] colliders = Physics2D.OverlapAreaAll(startingPos, endPos, 1 << 6, 1, 1);
            EnemyObjects.Clear();
            baseEnemies.Clear();
            foreach (Collider2D collider in colliders)
            {
                GameObject collisionGameObject = collider.gameObject;
                if (!EnemyObjects.Contains(collisionGameObject))
                {
                    BaseEnemy baseEnemy = collisionGameObject.GetComponent<BaseEnemy>();
                    if (baseEnemy != null)
                    {
                        EnemyObjects.Add(collisionGameObject);
                        baseEnemies.Add(baseEnemy);
                    }
                }
            }
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
