using UnityEngine;
/// <summary>
/// Clean up class responsible for deactivating all objects when (re)starting a level
/// Attached to the parent object of all object poolers
/// </summary>
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
    /// <summary>
    /// Sends a signal to all object poolers(children) to deactivate all objects
    /// Called every time a level is (re)loaded
    /// </summary>
    public void CleanUpObjects()
    {
        foreach (ObjectPooling pooler in GetComponentsInChildren<ObjectPooling>())
        {
            pooler.DeactivateAll();
        }
    }
}
