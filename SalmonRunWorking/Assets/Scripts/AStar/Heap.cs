using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is a Generic Minimum Heap Class
 * 
 * Primarily, its purpose is optimization of the A* Pathfinding Algorithm. However, by letting it accept a generic type,
 * T, this Heap Class may be applied to any type of heap, not just Nodes.
 * 
 * Here is a nice page explaining how Heaps generally work: https://www.geeksforgeeks.org/heap-data-structure/
 * 
 * Another bonus to this Heap Class Script is that you can save it and just import it into other projects if you ever need a Heap!
 */

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;              ///< The array of all the items in the heap
    int currentItemCount;   ///< The total number of items currently in the heap

    /*
     * Constructor
     */
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    /*
     * Adds an item to the heap, utilizing the IHeapItem interface so we can make use of general comparison functions from IComparable
     * \param item The item we would like to add to the heap
     */
    public void AddItem(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortItemInHeapUp(item);
        currentItemCount++;
    }

    /*
     * Removes an item from the Heap, utilizing the IHeapItem interface so we can make use of general comparison functions from IComparable
     * \return T The item that was just removed
     */
    public T RemoveItem()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortItemInHeapDown(items[0]);
        return firstItem;
    }

    /*
     * Sort an item in the heap downward accordingly to ensure the requirements of a minimum heap are currently met
     * \param item The item we would like to sort in the heap
     */
    private void SortItemInHeapDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    SwapItems(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    /*
     * Takes the given item in the heap and sorts it upwards accordingly to ensure the requirements of a minimum heap are currently met
     * \param item The item we would like to sort within the heap
     */
    private void SortItemInHeapUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        // This loop will continue until the item has been sorted into its proper place
        // (When it is lower in value than both of its children)
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                SwapItems(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    /*
     * Swaps the two provided items around in the heap. This swap is done between a parent and a child item in the heap
     * (in other words I would not use this to swap the root node with a node 3 layers down)
     * \param itemA The first item we would like to swap
     * \param itemB The second item we would like to swap
     */
    private void SwapItems(T itemA, T itemB)
    {
        // Swap the literal objects in the array
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        // Update each items index (we use a temporary itemAIndex to prevent A's original index from being lost when we swap A's to B's)
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

    /*
     * Returns a boolean indicating if the given item is somewhere in the heap or not
     * \param item The item in the heap we are searching for
     * \return bool Is the item in the heap (true) or not (false)?
     */
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    /*
     * Getter for the currentItemCount
     * \return int The value of currentItemCount
     */
    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    /*
     * Updates an item in the heap by sorting it (such as when we have a node that may change its fCost to be lower for A*)
     * \param item The item we are upadting
     */
    public void UpdateItem(T item)
    {
        SortItemInHeapUp(item);
    }
}

/*
 * Interface for each Heap Item that implements IComparable
 * 
 * This is needed mainly because this class is being made generically. As a result, we cannot assume that
 * any type of item being made into a heap will be as simple as saying x == y?
 * IComparable (which comes from "Using System" above) has functions such as CompareTo to allow us the needed flexibility
 */
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}