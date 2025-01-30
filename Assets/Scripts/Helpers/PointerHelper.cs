using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public static class PointerHelper
{
    /// <summary>
    /// Checks if pointer is hovering over a UI object
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns>True if it is</returns>
    public static bool PointerOverUI(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        for (int index = 0; index < raycastResults.Count; index++)
        {
            RaycastResult curRaysastResult = raycastResults[index];
            if (curRaysastResult.gameObject.layer == 5)//5 is UI value
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Checks if pointer is hovering over a Tower object
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns>True if it is</returns>
    public static bool PointerOverTower(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        for (int index = 0; index < raycastResults.Count; index++)
        {
            RaycastResult curRaysastResult = raycastResults[index];
            Debug.Log(curRaysastResult.gameObject.name);
            if (curRaysastResult.gameObject.layer == 3)//3 is Tower value
            {
                return true;
            }
        }
        return false;
    }

}
