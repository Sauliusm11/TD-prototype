using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap
{
    int[] heapArray;
    public int currentHeapSize;
    public int heapCapacity;
    public MinHeap(int n)
    {
        heapCapacity = n;
        heapArray = new int[n];
        currentHeapSize = 0;
    }
    public static void Swap<T>(ref T left, ref T right)
    {
        T temp = left;
        left = right;
        right = temp;
    }
    // Get the Parent index for the given index 
    public int Parent(int index)
    {
        return (index - 1) / 2;
    }
    // Get the Left Child index for the given index 
    public int Left(int index)
    {
        return 2 * index + 1;
    }
    // Get the Right Child index for the given index 
    public int Right(int index)
    {
        return 2 * index + 2;
    }
    // Returns the minimum key (key at 
    // root) from min heap  
    public int getMin()
    {
        return heapArray[0];
    }

    // Method to remove minimum element  
    // (or root) from min heap  
    public int extractMin()
    {
        if (currentHeapSize <= 0)
        {
            return int.MinValue;
        }

        if (currentHeapSize == 1)
        {
            currentHeapSize--;
            return heapArray[0];
        }

        // Store the minimum value,  
        // and remove it from heap  
        int root = heapArray[0];

        heapArray[0] = heapArray[currentHeapSize - 1];
        currentHeapSize--;
        MinHeapify(0);

        return root;
    }
    // A recursive method to heapify a subtree  
    // with the root at given index  
    // This method assumes that the subtrees 
    // are already heapified 
    public void MinHeapify(int key)
    {
        int l = Left(key);
        int r = Right(key);

        int smallest = key;
        if (l < currentHeapSize &&
            heapArray[l] < heapArray[smallest])
        {
            smallest = l;
        }
        if (r < currentHeapSize &&
            heapArray[r] < heapArray[smallest])
        {
            smallest = r;
        }

        if (smallest != key)
        {
            Swap(ref heapArray[key],
                 ref heapArray[smallest]);
            MinHeapify(smallest);
        }
    }
}
