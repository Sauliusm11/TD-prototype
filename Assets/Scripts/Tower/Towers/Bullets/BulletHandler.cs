using System.Collections;
using UnityEngine;
/// <summary>
/// Bullet handler class
/// </summary>
public class BulletHandler : MonoBehaviour
{
    ObjectPooling bulletPooler;
    // Start is called before the first frame update
    void Start()
    {
        bulletPooler = GameObject.Find("CannonBulletPooler").GetComponent<ObjectPooling>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Public facing method to start the bullet travelling coroutine
    /// </summary>
    /// <param name="targetObject">Target enemy to trace</param>
    /// <param name="bullet">Bullet prefab</param>
    /// <param name="projectileSpeed">Projectile speed of the tower that is shooting</param>
    /// <param name="damage">Damage the bullet will deal to the enemy</param>
    public void ShootBullet(GameObject targetObject, GameObject bullet, float projectileSpeed, int damage)
    {
        StartCoroutine(MoveBulletTo(targetObject, bullet, projectileSpeed,damage));
    }

    /// <summary>
    /// Coroutine which moves the bullet to the target
    /// </summary>
    /// <param name="target">Object the bullet is targeting</param>
    /// <param name="bullet">Bullet object</param>
    /// <param name="speed">Speed at which the bullet moves (should always be faster than fastest enemy)</param>
    /// <returns></returns>
    IEnumerator MoveBulletTo(GameObject target, GameObject bullet, float speed, int damage)
    {
        if (bulletPooler == null)
        {
            yield return null;
        }
        Vector3 oldPos = bullet.transform.position;
        Vector3 goTo;
        if (target != null)
        {
            goTo = target.transform.position;
            Vector3 path = oldPos - goTo;//This is close but not quite right (I might need to flip what is left over after this operation)
            path *= -1;//Flipping because we are -1 away from destination
            float totalTime = 1f;//Time in seconds at which a base speed unit crosses a base speed tile.
            if (speed != 0f)
            {
                totalTime /= speed;
                float timeElapsed = Time.deltaTime;
                do
                {
                    if (target != null)
                    {
                        goTo = target.transform.position;
                    }
                    else
                    {
                        bulletPooler.DeactivateObject(bullet);
                        //Destroy(bullet);
                        break;
                    }
                    float timeDelta = Time.deltaTime;
                    timeElapsed += timeDelta;
                    bullet.transform.position += path * (timeDelta / totalTime);
                    if (timeElapsed / totalTime > 1)
                    {
                        bullet.transform.position = goTo;
                        bulletPooler.DeactivateObject(bullet);
                        //Destroy(bullet);
                        BaseEnemy enemy = target.GetComponent<BaseEnemy>();
                        enemy.ReduceHealth(damage);

                    }
                    yield return null;
                } while (timeElapsed < totalTime);
            }
        }
        else
        {
            bulletPooler.DeactivateObject(bullet);
            //Destroy(bullet);
        }

    }
}
