using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made using https://www.geeksforgeeks.org/priority-queue-using-binary-heap/
//as reference
public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;
    public PriorityQueue()
    {
        data = new List<T>();
    }
    public void Enqueue(T item)
    {
        // item Insertion
        data.Add(item);
        int currentIndex = data.Count - 1;

        // re-structure heap(Max Heap) so that after addition max element will lie on top of pq
        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;
            if (data[currentIndex].CompareTo(data[parentIndex]) >= 0) //Finished heapifyinig
            {
                break; 
            }
            (data[parentIndex], data[currentIndex]) = (data[currentIndex], data[parentIndex]);
            //Original below
            //T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            currentIndex = parentIndex;
        }
    }
    public T Dequeue()
    {
        // deleting top element of pq
        int lastIndex = data.Count - 1;
        T firstItem = data[0];
        //Doing this ensures that remove at is O(1) instead of O(n), but makes resturcturing always? O(logn)
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);

        lastIndex--;
        int parentIndex = 0;

        // re-structure heap(Max Heap) so that after deletion max element will lie on top of pq
        while (true)
        {
            int currentIndex = parentIndex * 2 + 1;
            if (currentIndex > lastIndex)// No more children
            {
                break;
            }
            int rightChild = currentIndex + 1;
            if (rightChild <= lastIndex && data[rightChild].CompareTo(data[currentIndex]) < 0)
            { 
                currentIndex = rightChild; 
            }
            if (data[currentIndex].CompareTo(data[parentIndex]) >= 0) //Finished heapifyinig
            { 
                break; 
            }
            (data[currentIndex], data[parentIndex]) = (data[parentIndex], data[currentIndex]);
            //Original below
            //T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
            parentIndex = currentIndex;
        }
        return firstItem;
    }
    public T Peek()
    {
        return data[0];
    }
    public int Count()
    {
        return data.Count;
    }
}
