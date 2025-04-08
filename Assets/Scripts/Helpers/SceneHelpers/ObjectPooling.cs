using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Object pooling class used for objects that need to be instantiated often
/// </summary>
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
        //Pre-generating objects(does not work for certain types that have on start code dependent on other conditions)
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
    /// <summary>
    /// Activates an available object at the given transform
    /// If none are available, instantiates a new one
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>Gameobject that was activated/created</returns>
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
    /// <summary>
    /// Activates an available object at the given transform and sets the transform object as parent
    /// If none are available, instantiates a new one
    /// </summary>
    /// <param name="transform"></param>
    /// <returns>Gameobject that was activated/created</returns>
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
    /// <summary>
    /// Activates an available object at the given position and rotation
    /// If none are available, instantiates a new one
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns>Gameobject that was activated/created</returns>
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
    /// <summary>
    /// Deactivates a given object and adds it to the queue of available objects
    /// </summary>
    /// <param name="gameObject">Object to deactivate</param>
    public void DeactivateObject(GameObject gameObject)
    {
        for(int i = 0; i < ObjectList.Count; i++)
        {
            if(gameObject.GetInstanceID() == ObjectList[i].GetInstanceID())
            {

                if (gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(false);
                    AvailableObjectIndexes.Enqueue(i);
                }
            }
        }
    }
    /// <summary>
    /// Deactivates all active objects that are available to the object pooler
    /// </summary>
    public void DeactivateAll()
    {
        for (int i = 0; i < ObjectList.Count; i++)
        {
            if (ObjectList[i].activeInHierarchy)
            {
                ObjectList[i].SetActive(false);
                AvailableObjectIndexes.Enqueue(i);
            }
        }
    }
}
