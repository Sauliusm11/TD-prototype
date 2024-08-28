using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField]
    int countToPool;
    [SerializeField]
    GameObject prefab;
    List<GameObject> ObjectList = new List<GameObject>();
    Queue<int> AvailableObjectIndexes = new Queue<int>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < countToPool; i++)
        {
            ObjectList.Add(Instantiate(prefab));
            ObjectList[i].SetActive(false);
            AvailableObjectIndexes.Enqueue(i);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject ActivateObject(Transform transform)
    {
        if (AvailableObjectIndexes.Count > 0)
        {
            GameObject gameObject = ObjectList[AvailableObjectIndexes.Dequeue()];
            gameObject.transform.position = transform.position;
            gameObject.SetActive(true);
            return gameObject;
        }
        else
        {
            GameObject gameObject = Instantiate(prefab,transform.position,transform.rotation);
            ObjectList.Add(gameObject);
            gameObject.SetActive(true);
            return gameObject;
        }
    }
    public GameObject ActivateObjectWithParent(Transform transform)
    {
        if (AvailableObjectIndexes.Count > 0)
        {
            GameObject gameObject = ObjectList[AvailableObjectIndexes.Dequeue()];
            gameObject.transform.position = transform.position;
            gameObject.SetActive(true);
            return gameObject;
        }
        else
        {
            GameObject gameObject = Instantiate(prefab, transform);
            ObjectList.Add(gameObject);
            gameObject.SetActive(true);
            return gameObject;
        }
    }
    public GameObject ActivateObject(Vector3 position,Quaternion rotation)
    {
        if (AvailableObjectIndexes.Count > 0)
        {
            GameObject gameObject = ObjectList[AvailableObjectIndexes.Dequeue()];
            gameObject.transform.position = position;
            gameObject.SetActive(true);
            return gameObject;
        }
        else
        {
            GameObject gameObject = Instantiate(prefab, position, rotation);
            ObjectList.Add(gameObject);
            gameObject.SetActive(true);
            return gameObject;
        }
    }
    public void DeactivateObject(GameObject gameObject)
    {
        for(int i = 0;i < ObjectList.Count; i++)
        {
            if(gameObject.GetInstanceID() == ObjectList[i].GetInstanceID())
            {
                gameObject.SetActive(false);
                AvailableObjectIndexes.Enqueue(i);
            }
        }
    }
    public void DeactivateAll()
    {
        for (int i = 0; i < ObjectList.Count; i++)
        {
            ObjectList[i].SetActive(false);
            AvailableObjectIndexes.Enqueue(i);
        }
    }
}
