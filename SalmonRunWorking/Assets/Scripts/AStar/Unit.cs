using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class to represent units (in our case fish) to move from one end of the path to the other 
 */

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;       ///< Rather than every frame, we will allow a path update no sooner than .2 seconds
    const float pathUpdateThreshold = 0.5f;     ///< Rather than every couple seconds, we will only allow a path update if the unit has moved a certain distance
    
    public Transform target;        ///< Position of the target location
    public float speed = 20f;       ///< The speed of the unit
    public float turnSpeed = 3f;    ///< The speed at which the unit turns
    public float turnDistance = 5f; ///< The distance over which this unit will turn towards the next node

    Path path;

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    /*
     * On a callback, the unit will take the path it has been given and begin moving towards the final location
     * \param newPath The path this unit has calculated to reach the end
     * \param pathSuccess Was the unit able to find a path to the end?
     */
    public void OnPathFound(Vector3[] waypoints, bool pathSuccess)
    {
        if (pathSuccess == true)
        {
            path = new Path(waypoints, transform.position, turnDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }    
    }

    /*
     * As the unit is moving, we want them to update their path to ensure they are moving along efficiently
     * This coroutine performs a check every frame (with some restraints) to perform that update
     */
    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

        float squareMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;
        Vector3 targetPosOld = target.position;
        
        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > squareMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                targetPosOld = target.position;
            }
        }
    }

    /*
     * Coroutine to move the unit in the direction of the next path waypoint
     */
    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[0]);
        
        while (followingPath == true)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                Quaternion endRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
            }

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
            path.DrawWithGizmos();
        }
    }
}
