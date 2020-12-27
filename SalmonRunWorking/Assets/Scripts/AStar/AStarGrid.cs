using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public bool displayPath;            ///< Diagnostic boolean telling OnDrawGizmos to draw the path
    public Transform player;            ///< The position of the player
    public LayerMask unwalkableMask;    ///< The Unwalkable layer mask to check for collisions with Unwalkable obstacles
    public Vector3 gridWorldSize;       ///< The size of the full grid in world space
    public float nodeRadius;            ///< The radius of a single node (we draw nodes as cubes here but check for collisions in a sphere)
    Node[,] grid;                       ///< The actual 2D array of Nodes

    private float nodeDiameter;         ///< The diameter of the a single node which we use to calculate node spacing on the grid
    private int gridSizeX, gridSizeY;   ///< The size of the full grid in the X and Y driections to calculate node spacing on the grid

    private void Awake()
    {
        // Initialize the diameter and grid sizes based on the Inspector variables
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        // Create the actual NodeGrid
        CreateGrid();
    }

    public List<Node> path;
    
    /*
     * This function is a built in Unity function to draw in Cubes where each Node is located in the grid
     * This is not necessary for the grid to be created, but it makes it much easier to visualize
     */
    private void OnDrawGizmos()
    {
        if (displayPath == true)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    /*
     * Takes an entities position in world space and determines where on the grid they are located
     * \return Node The Node most closely associated with that world position
     */
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        // Calculate how far along the X and Y directions of the grid the entity is
        float percentX = (worldPosition.x / gridWorldSize.x) + 0.5f;
        float percentY = (worldPosition.y / gridWorldSize.y) + 0.5f;
        // These clamps ensure that the function always returns a valid grid position rather than one that is way outside of the grid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Calculate which node the entity is on
        int x = Mathf.FloorToInt(Mathf.Min(gridSizeX * percentX, gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Min(gridSizeY * percentY, gridSizeY - 1));

        return grid[x, y];
    }

    /*
     * Creates a 2D Node grid based on the provided size in the Inspector
     */
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        // Arbitrarily grabbing the bottem left as a starting position to create the rest of the grid (from bottem left to upper right)
        Vector3 worldBottemLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.up * gridWorldSize.y / 2);

        // Loop through every position on the grid and create a Node there
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the position of the new node
                Vector3 worldPoint = worldBottemLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                // Should the Node be walkable by the A* Algorithm or not
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    /*
     * Pass in a node and grab a list of all the neighboring nodes (potential paths to the endNode)
     * \param node The node we want to check for neighbors
     * \return List<Node> The list of neighboring nodes
     */
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Check the 3 x 3 square around the node. In this case (-1,-1) would be the lower left and (1,1) would be the upper right of the square
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // (0,0) is not a neightbor. It is the node itself which we want to skip
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Are we within the bounds of the grid?
                if ((checkX >= 0 && checkX < gridSizeX) && (checkY >= 0 && checkY < gridSizeY))
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
}
