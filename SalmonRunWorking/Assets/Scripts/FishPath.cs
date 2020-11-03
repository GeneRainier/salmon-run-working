using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class FishPath : MonoBehaviour
{
    [SerializeField] private Vector3 initialPosition = Vector3.zero;
    private Dictionary<float, Vector3> path = new Dictionary<float, Vector3>();

    private void Awake()
    {
        AddKeyFrame(0f, initialPosition);
        
        #if UNITY_EDITOR
        //Test();
        #endif
    }

    #if UNITY_EDITOR
    private void Test()
    {
        Vector3 v = new Vector3(Random.Range(-10f,10f), Random.Range(-10f,10f), 0f);

        print("Velocity: " + v);

        int n = 10;
        
        for (int t = 0; t < n; t++)
        {
            float time = Random.Range(t / 10f, n);

            Vector3 pos = v * t;

            AddKeyFrame(time, pos);
        }

        float randomTime = 0.5f;//Random.Range(0f, n);
        Vector3 expectedPosition = v * randomTime;
        Vector3 calculatedPosition = GetKeyFrame(randomTime);

        print("Selected Time: " + randomTime);
        print("Expected Position: " + expectedPosition);
        print("Calculated Position: " + calculatedPosition);

        print(string.Join("\t", GetClosestTimes(randomTime, path.Count)));

        Assert.IsTrue(Mathf.Approximately(expectedPosition.x, calculatedPosition.x));
        Assert.IsTrue(Mathf.Approximately(expectedPosition.y, calculatedPosition.y));
        Assert.IsTrue(Mathf.Approximately(expectedPosition.z, calculatedPosition.z));
    }
    #endif


    public void AddKeyFrame(float time, Vector3 position)
    {
        path.Add(time, position);
    }

    public Vector3 GetKeyFrame(float time)
    {
        if (path.Keys.Count < 1) return Vector3.negativeInfinity;
        
        if (path.ContainsKey(time))
        {
            return path[time];
        }
        
        if (path.Keys.Count < 2)
        {
            return path[GetClosestTimes(time, 1)[0]];
        }

        float[] closestTimes = GetClosestTimes(time);

        return Vector3.Lerp(path[closestTimes[0]], path[closestTimes[1]], Diff(closestTimes[0], closestTimes[1]));
    }

    private float Diff(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    private float[] GetClosestTimes(float time, int amount = 2)
    {
        List<float> list = path.Keys.ToList();
        
        // Sort the List based on how close it is to the time
        list.Sort((x, y) => Diff(x, time).CompareTo(Diff(y, time)));

        // We assume there is at least one element
        return list.Take(amount).ToArray();
    }
}
