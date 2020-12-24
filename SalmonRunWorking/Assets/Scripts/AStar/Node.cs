﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is the class script for an A* Pathfinding Node.
 * 
 * It does not extend monobehavior as it does not need any of the baseline Unity classes such as Start, Awake, Update, etc.
 * 
 * The A* Pathfinding Algorithm works by having a grid of nodes which designate the area the algotithm is forming a path through.
 * The path it is looking for is designated by a start and end node. Each node on the grid has 3 values associated with it:
 * 
 * G Cost = The distance from the starting node
 * H Cost = The distance from the end node
 * F Cost = G Cost + H Cost
 * 
 * The Algorithm checks the initial state of the nodes and searches the node with the lowest F Cost. If the F Costs of multiple nodes are equal, then
 * the algorithm starts with the node with the lowest H Cost(the node supposedly closest to the end). Sometimes this means two nodes will appear identical
 * in their values and one will just be chosen at random. 
 * 
 * This algorithm will find the shortest path between the start and end nodes and can be modified to extend its basic capabilities (restrict certain types of nodes,
 * intermediate nodes, etc.)
 */

public class Node : IHeapItem<Node>
{
    public bool walkable;           ///< Is this Node a viable, accessible option for a path?
    public Vector3 worldPosition;   ///< The world position of the Node in the game scene
    public int gridX;               ///< The x position of the Node in the game scene
    public int gridY;               ///< The y position of the node in the game scene

    public int gCost;               ///< The distance of this Node from the starting Node
    public int hCost;               ///< The distance of this Node from the ending Node
    public Node parent;             ///< The node that this node derives its path from (i.e. the node before this one in the path)
    int heapIndex;                  ///< The index of this node within the Node Heap

    /*
     * Constructor
     */
    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    /*
     * Getter function for the fCost (We could have a general variable, but we never assign fCost. We just need to calculate it as we go)
     */
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    /*
     * Implementation of HeapIndex from the Heap class interface
     * 
     * THIS MUST BE HERE! Implementing the Interface here means we need to give specific implementations for what HeapIndex and CompareTo look like in a specific context
     * (remember that the Heap class is generic). Without providing that information here, there is no way for the compiler to understand how it should handle Nodes when
     * we bring up .HeapIndex  or CompareTo in or code.
     */
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    /*
     * Implementation of CompareTo from the Heap class interface and the IComparable interface
     * 
     * THIS MUST BE HERE! Implementing the Interface here means we need to give specific implementations for what HeapIndex and CompareTo look like in a specific context
     * (remember that the Heap class is generic). Without providing that information here, there is no way for the compiler to understand how it should handle Nodes when
     * we bring up .HeapIndex  or CompareTo in or code.
     */
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}