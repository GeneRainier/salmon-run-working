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
    
    public Transform target;        ///< Position of the final, target location
    public Transform[] targets;     ///< An array of the possible end points a unit may choose to end at
    public Transform intermediateNode;          ///< Position if the required mid-point in the unit's path
    public Transform[] intermediateNodes;       ///< An array of all the positions this unit may choose to visit before heading for the target
    [SerializeField] private bool intermediateReached = false;   ///< Has this unit reached the intermediate node yet?
    public float speed = 20f;       ///< The speed of the unit
    public float turnSpeed = 3f;    ///< The speed at which the unit turns
    public float turnDistance = 5f; ///< The distance over which this unit will turn towards the next node

    [SerializeField] private AStarGrid theGrid;     ///< A reference to the grid so we can compare node positions for the player, intermediate node, and target
    Path path;      ///< The path script this unit is following to smooth its movement

    private void Start()
    {
        // Choose the intermediate node and target node to head towards prior to the final target and begin following the path
        int intermediateIndex = Random.Range(0, intermediateNodes.Length);
        int targetNodeIndex = Random.Range(0, targets.Length);
        intermediateNode = intermediateNodes[intermediateIndex];
        target = targets[targetNodeIndex];
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        // Have we reached the intermediate node?
        if (Vector3.Distance(transform.position, intermediateNode.position) <= 3)
        {
            intermediateReached = true;
        }
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
        
        // If we have not yet visited the intermediate node, then we should continue pathing to there.
        // Otherwise, we start pathing towards the final node
        PathRequestManager.RequestPath(new PathRequest(transform.position, intermediateNode.position, OnPathFound));

        float squareMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;
        Vector3 targetPosOld = intermediateNode.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if (((intermediateNode.position - targetPosOld).sqrMagnitude > squareMoveThreshold) && intermediateReached == false)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, intermediateNode.position, OnPathFound));
                targetPosOld = intermediateNode.position;
            }
            else if (((target.position - targetPosOld).sqrMagnitude > squareMoveThreshold) && intermediateReached == true)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
        //else
        //{
        //    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        //    float squareMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;
        //    Vector3 targetPosOld = target.position;

        //    while (true)
        //    {
        //        yield return new WaitForSeconds(minPathUpdateTime);
        //        if ((target.position - targetPosOld).sqrMagnitude > squareMoveThreshold)
        //        {
        //            PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        //            targetPosOld = target.position;
        //        }
        //    }
        //}
        //PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        //float squareMoveThreshold = pathUpdateThreshold * pathUpdateThreshold;
        //Vector3 targetPosOld = target.position;
        
        //while (true)
        //{
        //    yield return new WaitForSeconds(minPathUpdateTime);
        //    if ((target.position - targetPosOld).sqrMagnitude > squareMoveThreshold)
        //    {
        //        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        //        targetPosOld = target.position;
        //    }
        //}
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
