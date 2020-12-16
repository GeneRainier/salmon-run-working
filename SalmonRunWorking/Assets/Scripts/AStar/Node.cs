using System.Collections;
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

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;

    public Node(bool _walkable, Vector3 _worldPosition)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
    }
}
