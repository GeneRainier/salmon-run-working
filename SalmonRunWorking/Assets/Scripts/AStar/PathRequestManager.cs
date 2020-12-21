using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is dedicated to processing requests from individual units (in our case fish) to calculate a path from one end of the map to the other.
 * 
 * It utilizes a queue which you can find more informtaion about here: https://www.geeksforgeeks.org/c-sharp-queue-class/
 * We do this to create a structured list of the pathfinding requests which can be added to by newly spawned units.
 */

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequests = new Queue<PathRequest>();     ///< The queue of path requests
    PathRequest currentPathRequest;                                 ///< The path request currently being processed

    static PathRequestManager instance;                             ///< The instance of the PathRequestManager script in the scene
    private Pathfinding pathfinding;                                ///< A reference to the Pathfinding script

    bool isProcessing;                                              ///< Boolean tracking whether or not the path request (finding and setting path) is still being processed

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    /*
     * Add a request for a path from pathStart to pathEnd
     * \param pathStart The start of the path to be found
     * \param pathEnd The end of the path to be found
     */
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequests.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    /*
     * Remove the path request from the queue and begin the process of finding the path in the Pathfinding script
     */
    private void TryProcessNext()
    {
        if (isProcessing == false && pathRequests.Count > 0)
        {
            currentPathRequest = pathRequests.Dequeue();
            isProcessing = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    /*
     * Notify the manager that the prior path in the queue is finished processing and start processing the next request
     * \param path An array of the full path
     * \param success Was there a path successfully found?
     */
    public void FinishedProcessing(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessing = false;
        TryProcessNext();
    }

    /*
     * C# data structure to represent a PathRequest (these will be placed into the queue)
     */
    struct PathRequest
    {
        public Vector3 pathStart;                   ///< The beginning of the desired path
        public Vector3 pathEnd;                     ///< The end of the desired path
        public Action<Vector3[], bool> callback;    ///< System Action that reports pathfinding success and the path found as Vector 3 positions

        /*
         * Constructor for a PathRequest
         */
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }    
}
