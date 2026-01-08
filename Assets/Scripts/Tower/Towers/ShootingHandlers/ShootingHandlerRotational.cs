using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tower.Towers
{
    public class ShootingHandlerRotational : ShootingHandler
    {
        public override void AimAtTarget()
        {
            //Update target list before picking target
            UpdateTargets();
            float closestToExit = float.MaxValue;//Flip for last
            int closestIndex = 0;
            if (baseEnemies.Count > 0)
            {
                for (int i = 0; i < baseEnemies.Count; i++)
                {
                    float progress = baseEnemies[i].GetProgress();
                    if (progress < closestToExit)//Flip for last
                    {
                        closestIndex = i;
                        closestToExit = progress;
                    }
                }
                currentTarget = closestIndex;
                GameObject targetObject = EnemyObjects[currentTarget];
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

        public override void Shoot()
        {
            //Update target right before shooting
            AimAtTarget();
            if (currentTarget > -1)
            {
                GameObject targetObject = EnemyObjects[currentTarget];
                GameObject bullet = bulletPooler.ActivateObject(shootingPoint.transform);
                bullet.GetComponent<BulletHandler>().ShootBullet(targetObject, bullet, projectileSpeed, damage);
                timeSinceShot = 0;
            }
        }

        public override void UpdateTargets()
        {
            //Change the second one to 2 for ground+air guns and both for air only guns?
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, range, 1 << 6, 1, 1);
            //There has to be a better way, right?
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
    }
}
