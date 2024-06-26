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
        Vector3Int target = gameManager.GetCastleLocation();
        Vector3 current = transform.position;
        goTo = current;
        int counter = 0;
        while (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y) > 0 && counter < 1000)
        {
            counter++;
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
            //TODO: Call moveTo and wait for it
            yield return null;
        }

        Debug.Log(counter);
    }
    IEnumerator MoveTo(Vector3 goTo, float time)
    {
        //TODO: Implement moving
        yield return null;
    }
}
