using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

/*
 * This is the script to actually calculate the pathfinding for the A* algorithm
 */

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    
    AStarGrid grid;     ///< The grid created in the AStarScript which defines the bounds of our scene, where is walkable, where is unwalkable, etc.

    private void Awake()
    {
        // Grab the grid created by the AStarGrid script (Both scripts must be on the same object for this to work!)
        grid = GetComponent<AStarGrid>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            FindPath(seeker.position, target.position);
        }
    }

    /*
     * Find the shortest path from the startNode to the endNode
     * \param startPos The starting position of the path
     * \param endPos The ending position of the path
     */
    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // Initialize the start and end Nodes to be the closest Nodes to the starting and ending positions
        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node endNode = grid.GetNodeFromWorldPoint(endPos);

        // Nodes we have yet to evaluate. We are using a Heap for the sake of optimization since it will make comparisons FAR faster on a large scale
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        /* Nodes we have evaluated. We are using a HashSet since we are checking this list often and HashSets have constant time complexity for 
           adding and removing items and checking current size is constant (Good news for large maps like ours) */
        HashSet<Node> closedSet = new HashSet<Node>();
        // Initialize the openSet list
        openSet.AddItem(startNode);

        // While we are still searching Nodes to find the best path
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveItem();
            
            closedSet.Add(currentNode);
            // Have we found the endNode? If so, we are done and the path has been found
            if (currentNode == endNode)
            {
                sw.Stop();
                print("Path Found in " + sw.ElapsedMilliseconds + "ms");
                RetracePath(startNode, endNode);
                return;
            }

            // Check every neighbor to the current node and calculate values (g and h Costs and parent nodes)
            foreach(Node neighbor in grid.GetNeighbors(currentNode))
            {
                // If the neighboring node is not valid, we can just skip it
                if (neighbor.walkable == false || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || openSet.Contains(neighbor) == false)
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    // Each of these neighbor nodes will need to be evaluated, so add them to the openSet
                    if (openSet.Contains(neighbor) == false)
                    {
                        openSet.AddItem(neighbor);
                    }
                }
            }
        }
    }

    /*
     * Take the start and end Nodes and grab the path between them based on FindPath
     * \param startNode The node where the path starts
     * \param endNode The node where the path ends
     */
    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // The list we have is going from end to start. We want the reverse of that
        path.Reverse();

        grid.path = path;
    }

    /*
     * Finds the Distance between two Nodes
     * \param nodeA The first node
     * \param nodeB The second node
     * \return int The distance between the two nodes
     */
   private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
