using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinNodeHeap
{
    public MinNodeHeap(int startingSize = 15)
    {
        if(startingSize % 2 != 1)
        {
            startingSize++;
        }
        array = new HeuristicPosPair[startingSize];

        for (int i = 0; i < array.Length; i++)
        {
            array[i].heuristic = int.MaxValue;
        }
    }

    private static void Swap(ref HeuristicPosPair a, ref HeuristicPosPair b)
    {
        HeuristicPosPair temp;
        temp = a;
        a = b;
        b = temp;
    }

    private static int GetParentIndex(int index)
    {
        return (index - 1) / 2;
    }

    private static int GetChildIndex_1(int index)
    {
        return index * 2 + 1;
    }

    private static int GetChildIndex_2(int index)
    {
        return index * 2 + 2;
    }

    private HeuristicPosPair[] array;

    private int currentSize = 0;

    private void ResizeArray()
    {
        HeuristicPosPair[] temp = array;
        int previousLength = temp.Length;
        array = new HeuristicPosPair[array.Length * 2 + 1];
        temp.CopyTo(array, 0);

        for (int i = previousLength; i < array.Length; i++)
        {
            array[i].heuristic = int.MaxValue;
        }
    }

    public void Insert(HeuristicPosPair node)
    {        
        array[currentSize] = node;

        HeapifyUp(currentSize);
        currentSize++;

        if (array.Length == currentSize)
        {
            ResizeArray();
        }
    }

    public HeuristicPosPair PeekMin()
    {
        return array[0];
    }

    public HeuristicPosPair ExtractMin()
    {
        HeuristicPosPair minNode = array[0];

        currentSize--;
        array[0] = array[currentSize];
        array[currentSize].heuristic = int.MaxValue;

        HeapifyDown(0);

        return minNode;
    }

    private void HeapifyUp(int index)
    {
        int parentIndex = GetParentIndex(index);
        if (array[index].heuristic < array[parentIndex].heuristic)
        {
            Swap(ref array[index], ref array[parentIndex]);
            HeapifyUp(parentIndex);
        }
    }

    private void HeapifyDown(int index)
    {
        int childIndex_1 = GetChildIndex_1(index);
        int childIndex_2 = GetChildIndex_2(index);

        if (childIndex_2 >= array.Length)
        {
            return;
        }

        int smallestIndex = array[childIndex_1].heuristic < array[childIndex_2].heuristic ? childIndex_1 : childIndex_2;

        if (array[index].heuristic > array[smallestIndex].heuristic)
        {
            Swap(ref array[index], ref array[smallestIndex]);
            HeapifyDown(smallestIndex);
        }
    }


    public override string ToString()
    {
        if (array[0].heuristic == int.MaxValue)
        {
            return "empty";
        }

        string returnString = array[0].ToString();

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i].heuristic == int.MaxValue)
            {
                break;
            }

            returnString += $"; {array[i]}";
        }

        return returnString;
    }
}
