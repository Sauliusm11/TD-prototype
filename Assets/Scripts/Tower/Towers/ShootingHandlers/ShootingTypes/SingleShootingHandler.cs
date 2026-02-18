using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers
{
    public class SingleShootingHandler : ShootingHandler
    {
        [SerializeField]
        GameObject bulletPrefab;
        GameManager.TargetingType targetingType;
        PriorityQueue<BaseEnemy> priorityQueue = new PriorityQueue<BaseEnemy>();
        List<BaseEnemy> sortedEnemyList = new List<BaseEnemy>();
        public override void AimAtTarget()
        {
            //Update target list before picking target
            baseEnemies = targetingHandler.UpdateTargets(range);
            SortBasedOnProgress();
            switch (targetingType)
            {
                case GameManager.TargetingType.First:
                    AimBasedOnProgress(true, false);
                    break;
                case GameManager.TargetingType.Last:
                    AimBasedOnProgress(false, false);
                    break;
                case GameManager.TargetingType.Strong:
                    AimBasedOnProgress(false, true);
                    break;
                case GameManager.TargetingType.Weak:
                    AimBasedOnProgress(true, true);
                    break;
                default:
                    break;
            }
        }

        public override void Shoot()
        {
            //Update target right before shooting
            AimAtTarget();
            if (currentTarget > -1)
            {
                GameObject targetObject = sortedEnemyList[currentTarget].gameObject;
                GameObject bullet = bulletPooler.ActivateObject(shootingPoint.transform);
                bullet.GetComponent<BulletHandler>().ShootBullet(targetObject, bullet, projectileSpeed, damage);
                timeSinceShot = 0;
            }
        }
        public override GameManager.TargetingType GetTargetingType()
        {
            return targetingType;
        }
        public override void SetTargetingType(GameManager.TargetingType newTargetingType)
        {
            targetingType = newTargetingType;
        }
        private void SortBasedOnProgress()
        {
            if (baseEnemies.Count > 0)
            {
                for (int i = 0; i < baseEnemies.Count; i++)
                {
                    priorityQueue.Enqueue(baseEnemies[i]);
                }
                sortedEnemyList.Clear();
                for (int i = 0; i < baseEnemies.Count; i++)
                {
                    sortedEnemyList.Add(priorityQueue.Dequeue());
                }
            }
        }
        private void AimBasedOnProgress(bool first, bool health)
        {
            float currentTargetValue = first ? float.MaxValue : float.MinValue;
            int closestIndex = 0;
            if (!first && !health)
            {
                closestIndex = sortedEnemyList.Count - 1;
            }

            if (baseEnemies.Count > 0)
            {
                if (health)
                {
                    for (int i = 0; i < sortedEnemyList.Count; i++)
                    {
                        float currentHealth = sortedEnemyList[i].GetHealth();
                        if (first && currentHealth < currentTargetValue)
                        {
                            closestIndex = i;
                            currentTargetValue = currentHealth;
                        }
                        if (!first && currentHealth > currentTargetValue)
                        {
                            closestIndex = i;
                            currentTargetValue = currentHealth;
                        }
                    }
                }
                currentTarget = closestIndex;
                GameObject targetObject = sortedEnemyList[currentTarget].gameObject;
                //Rotates the gun to point at the target
                Vector2 direction = partToRotate.transform.position - targetObject.transform.position;
                Quaternion rotation = new Quaternion();
                rotation.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90);//Idk why, but + 90 helps
                partToRotate.transform.rotation = rotation;
            }
            else
            {
                currentTarget = -1;
            }
        }
    }
}
