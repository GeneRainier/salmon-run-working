using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector3[] lookPoints;       ///< A rebranding of the waypoints array found in the PathRequestManager
    public readonly Line[] turnBoundaries;      ///< An array of Line Structs from the Line script that will determine when an entity should turn
    public readonly int finishLineIndex;        ///< The index of the end location in the path so we can ensure the entity does not turn when it reaches the end

    /*
     * Constructor for a smoother Path
     * This constructor does a lot of conversions from Vector3 to Vector 2 since we are just interested in individual points in the
     * waypoints array that we will be measuring distances between
     */
    public Path(Vector3[] waypoints, Vector3 startPos, float turnDistance)
    {
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = V3toV2(startPos);
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = V3toV2(lookPoints[i]);
            Vector2 directionToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - directionToCurrentPoint * turnDistance;
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - directionToCurrentPoint * turnDistance);
            previousPoint = turnBoundaryPoint;
        }
    }

    /*
     * A function to convert from a Vector3 to a Vector2. Used throughout this script for convencience
     * \param v3 The Vector3 we are converting
     * \return Vector2 The converted Vector3
     */
    private Vector2 V3toV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
    
    /*
     * A Diagnostic function that allows us to draw the line in the game scene as the entity following the path is moving
     */
    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 point in lookPoints)
        {
            Gizmos.DrawCube(point + Vector3.up, Vector3.one);
        }

        Gizmos.color = Color.white;
        foreach (Line l in turnBoundaries)
        {
            l.DrawWithGizmos(10);
        }
    }
}
