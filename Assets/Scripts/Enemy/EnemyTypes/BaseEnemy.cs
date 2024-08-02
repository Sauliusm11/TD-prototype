using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for the basic movement enemy type (might become abstract later :p)
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    EnemyPathFinding pathFinder;
    Stack<WorldNode> path;
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = GameObject.Find("BasicEnemyPathfinder").GetComponent<EnemyPathFinding>();
        path = pathFinder.GetPath();
        StartCoroutine(FollowPath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Move the enemy one tile at a time along the path
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        while (path.Count > 0)
        {
            WorldNode node = path.Pop();
            yield return StartCoroutine(MoveTo(node.GetVector3(), node.GetMovementSpeedCoef()));
        }
        yield return null;
    }
    /// <summary>
    /// Increment the movement of the enemy to a given world space
    /// </summary>
    /// <param name="goTo">End position of the enemy</param>
    /// <param name="speed">Speed at which the enemy 'walks'</param>
    /// <returns></returns>
    IEnumerator MoveTo(Vector3 goTo, float speed)
    {
        GameObject enemy = gameObject;
        Vector3 oldPos = enemy.transform.position;
        Vector3 path = oldPos - goTo;//This is close but not quite right (I might need to flip what is left over after this operation)
        path *= -1;//Flipping because we are -1 away from destination
        float totalTime = 1f;//Time in seconds at which a base speed unit crosses a base speed tile.
        if (speed != 0f)
        {
            totalTime /= speed;
            float timeElapsed = 0f;
            while (timeElapsed < totalTime)
            {
                float timeDelta = Time.deltaTime;
                timeElapsed += timeDelta;
                enemy.transform.position += path * (timeDelta / totalTime);
                if (timeElapsed / totalTime > 1)
                {
                    enemy.transform.position = goTo;
                }
                yield return null;
            }
        }
    }
}
