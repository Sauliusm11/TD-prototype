using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class providing universal useful methods
/// </summary>
public class Utility
{
    /// <summary>
    /// Float equals method to go around the problem of 2.000000000 != 2.000000001
    /// </summary>
    /// <param name="left">First number</param>
    /// <param name="right">Second number</param>
    /// <param name="irrelaventDifference">The difference we want to ignore, default 0.001</param>
    /// <returns></returns>
    public static bool Equals(float left, float right, float irrelaventDifference = 0.001F)
    {
        if (Mathf.Abs(left - right) < irrelaventDifference)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
