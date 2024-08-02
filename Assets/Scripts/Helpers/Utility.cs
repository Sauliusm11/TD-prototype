using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class providing universal useful methods
/// </summary>
public static class Utility
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
    /// <summary>
    /// Clones a stack while keeping the original order.
    /// From https://stackoverflow.com/a/45200965
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <returns></returns>
    public static Stack<T> CloneStack<T>(this Stack<T> original)
    {
        var arr = new T[original.Count];
        original.CopyTo(arr, 0);
        Array.Reverse(arr);
        return new Stack<T>(arr);
    }
}
