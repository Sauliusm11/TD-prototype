using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUp : MonoBehaviour
{
    TowerContainer towerContainer;
    // Start is called before the first frame update
    void Start()
    {
        towerContainer = TowerContainer.getInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CleanUpObjects()
    {
        foreach (ObjectPooling pooler in GetComponentsInChildren<ObjectPooling>())
        {
            pooler.DeactivateAll();
        }
    }
}
