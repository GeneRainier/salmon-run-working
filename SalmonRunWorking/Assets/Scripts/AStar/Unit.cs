using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class to represent units (in our case fish) to move from one end of the path to the other 
 */

public class Unit : MonoBehaviour
{
    public Transform target;        ///< Position of the target location
    private float speed = 20;       ///< The speed of the unit
    private Vector3[] path;         ///< The path this unit is taking
    private int targetIndex;        ///< The index of the path position the unit is currently moving towards

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    /*
     * On a callback, the unit will take the path it has been given and begin moving towards the final location
     * \param newPath The path this unit has calculated to reach the end
     * \param pathSuccess Was the unit able to find a path to the end?
     */
    public void OnPathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess == true)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }    
    }

    /*
     * Coroutine to move the unit in the direction of the next path waypoint
     */
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    /*
     * General diagnostic script that draws the path this unit is taking to its final path location
     */
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
