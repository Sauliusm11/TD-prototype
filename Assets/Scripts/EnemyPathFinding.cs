using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{
    GameManager gameManager;
    Vector3 goTo;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(CalculatePath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CalculatePath()
    {
        Vector3 target = gameManager.GetCastleLocation();
        Vector3 current = transform.position;
        goTo = current;
        int counter = 0;
        while (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y) > 0 && counter < 1000)
        {
            counter++;
            if (Utility.Equals(target.x, current.x)) 
            {
                if (target.y > current.y)
                {
                    goTo.y += 1;
                }
                else
                {
                    goTo.y -= 1;
                }
            }
            else
            {
                if (Utility.Equals(target.y, current.y)) 
                {
                    if (target.x > current.x)
                    {
                        goTo.x += 1;
                    }
                    else
                    {
                        goTo.x -= 1;
                    }
                }
                else
                {
                    if (Mathf.Abs(target.x - current.x) > Mathf.Abs(target.y - current.y))
                    {
                        if (target.x > current.x)
                        {
                            goTo.x += 1;
                        }
                        else
                        {
                            goTo.x -= 1;
                        }
                    }
                    else
                    {
                        if (target.y > current.y)
                        {
                            goTo.y += 1;
                        }
                        else
                        {
                            goTo.y -= 1;
                        }
                    }
                }
            }
            //TODO: Call moveTo and wait for it
            //Debug.Log(string.Format("current: {0}, goto: {1}, target: {2}", current, goTo, target));
            yield return StartCoroutine(MoveTo(goTo, 2));
            current = transform.position;
            goTo = current;
            //Debug.Log(string.Format("Current: {0}", current));
        }
    }
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
