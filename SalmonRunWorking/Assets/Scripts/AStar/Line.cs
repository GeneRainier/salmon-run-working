using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is focused around representing a special line struct. This struct will be given to each important waypoint in the path
 * found by the Pathfinding script in order to make any turning feel more smooth / natural.
 */

public struct Line
{
    const float verticalLineGradient = 1e5f;        ///< A constant to represent the gradient in the event dx (delta x) is 0 to avoid a division by 0
    
    private float gradient;             ///< The gradient of this line (how is it angled)
    private float yIntercept;           ///< The y intercept of the line
    private Vector2 pointOnLine_1;      ///< A point located on this line
    private Vector2 pointOnLine_2;      ///< Another point located on this line

    private float gradientPerpendicular;     ///< The gradient perpendicular to this line (how is it going to have to turn)

    private bool approachSide;          ///< The side that represents an entity approaching a node (we are either on one side or the other -> true / false

    /*
     * Constructor for this type of line struct
     */
    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;
        
        if (dx == 0)
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }

        if (gradientPerpendicular == 0)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }

        yIntercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(1, gradient);
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    /*
     * Determine which side of the line the given point falls on (true for one side and false otherwise)
     * \param point The point we are determining the location of
     * \return bool Which side of the line the point falls on
     */
    private bool GetSide(Vector2 point)
    {
        // 
        return (point.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (point.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    /*
     * Determine if the line for a node has been crossed so we can determine whether to begin turning or not
     * \param point The point we are determining the location of
     * \return bool Has the line been crossed (har har)?
     */
    public bool HasCrossedLine(Vector2 point)
    {
        return GetSide(point) != approachSide; 
    }

    /*
     * Defines a way for the Line struct to draw itself in the game scene so the Diagnostic Gizmo functions can work with it
     * \param length The length of the line we would like to draw in the scene
     */
    public void DrawWithGizmos(float length)
    {
        Vector3 lineDir = new Vector3(1, gradient, 0).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, pointOnLine_1.y, 0) + Vector3.up;
        Gizmos.DrawLine(lineCenter - lineDir * length/2f, lineCenter + lineDir * length/2f);
    }
}
