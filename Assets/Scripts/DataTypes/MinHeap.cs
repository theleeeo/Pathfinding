using UnityEngine;

public class MinHeap
{
    public MinHeap(int startingSize = 15)
    {
        if (startingSize % 2 != 1)
        {
            startingSize++;
        }

        array = new int[startingSize];

        for (int i = 0; i < startingSize; i++)
        {
            array[i] = int.MaxValue;
        }
    }

    private static void Swap(ref int a, ref int b)
    {
        int temp;
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

    private int[] array;

    private int currentSize = 0;

    private void ResizeArray()
    {
        int[] temp = array;
        int previousLength = temp.Length;
        array = new int[previousLength * 2 + 1];
        temp.CopyTo(array, 0);

        for (int i = previousLength; i < array.Length; i++)
        {
            array[i] = int.MaxValue;
        }
    }

    public void Insert(int value)
    {
        if(array.Length == currentSize)
        {
            ResizeArray();
        }

        array[currentSize] = value;

        HeapifyUp(currentSize);
        currentSize++;
    }

    public int PeekMin()
    {
        return array[0];
    }

    public int ExtractMin()
    {
        int minValue = array[0];

        currentSize--;
        array[0] = array[currentSize];
        array[currentSize] = int.MaxValue;


        HeapifyDown(0);

        return minValue;
    }

    private void HeapifyUp(int index)
    {
        int parentIndex = GetParentIndex(index);
        if (array[index] < array[parentIndex])
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

        int smallestIndex = array[childIndex_1] < array[childIndex_2] ? childIndex_1 : childIndex_2;

        if(array[index] > array[smallestIndex])
        {
            Swap(ref array[index], ref array[smallestIndex]);
            HeapifyDown(smallestIndex);
        }
    }


    public override string ToString()
    {
        string returnString = array[0].ToString();

        for (int i = 1; i < array.Length; i++)
        {
            returnString += ", " + array[i];
        }

        return returnString;
    }
}
