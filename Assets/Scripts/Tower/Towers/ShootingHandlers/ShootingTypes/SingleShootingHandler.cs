using UnityEngine;

namespace Assets.Scripts.Tower.Towers
{
    public class SingleShootingHandler : ShootingHandler
    {
        [SerializeField]
        GameObject bulletPrefab;
        public override void AimAtTarget()
        {
            //Update target list before picking target
            baseEnemies = targetingHandler.UpdateTargets(range);
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
                GameObject targetObject = baseEnemies[currentTarget].gameObject;
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
                GameObject targetObject = baseEnemies[currentTarget].gameObject;
                GameObject bullet = bulletPooler.ActivateObject(shootingPoint.transform);
                bullet.GetComponent<BulletHandler>().ShootBullet(targetObject, bullet, projectileSpeed, damage);
                timeSinceShot = 0;
            }
        }
    }
}
