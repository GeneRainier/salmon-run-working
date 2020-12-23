using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/*
 * This script is dedicated to processing requests from individual units (in our case fish) to calculate a path from one end of the map to the other.
 * 
 * It utilizes a queue which you can find more informtaion about here: https://www.geeksforgeeks.org/c-sharp-queue-class/
 * We do this to create a structured list of the pathfinding requests which can be added to by newly spawned units.
 */

public class PathRequestManager : MonoBehaviour
{
    Queue<PathResult> results = new Queue<PathResult>();
    
    static PathRequestManager instance;                             ///< The instance of the PathRequestManager script in the scene
    private Pathfinding pathfinding;                                ///< A reference to the Pathfinding script

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock(results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    /*
     * Add a request for a path from pathStart to pathEnd
     * \param request The newest request from a GameObject for a path
     */
    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(request, instance.FinishedProcessing);
        };
        threadStart.Invoke();
    }

    /*
     * Notify the manager that the prior path in the queue is finished processing and start processing the next request
     * \param path An array of the full path
     * \param success Was there a path successfully found?
     * \param originalRequest The PathRequest that was called in RequestPath to start the thread
     */
    public void FinishedProcessing(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
        
    }
}

/*
 * C# data structure to represent a PathResult (these will be placed into the queue)
 */
public struct PathResult
{
    public Vector3[] path;                      ///< The path that was found for a certain request
    public bool success;                        ///< The success of finding a path for a certain request
    public Action<Vector3[], bool> callback;    ///< System Action that reports pathfinding success and the path found as Vector 3 positions

    /*
     * Constructor for a PathResult
     */
    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

/*
 * C# data structure to represent a PathRequest
 */
public struct PathRequest
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