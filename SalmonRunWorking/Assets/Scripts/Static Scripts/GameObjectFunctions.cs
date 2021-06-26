using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Functions that are utilized across GameObjects for easier calculations and movement
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public static class GameObjectFunctions
{
    /*
     * Smoothly moves a Gameobject from the start position to the destination position at a constant speed
     * 
     * @param transform The start position
     * @param destination The end position
     * @param moveSpeed The speed to move towards the destination
     */
    public static void SmoothMoveTowards(this Transform transform, Vector3 destination, float moveSpeed)
    {
        transform.position = Vector3.Lerp(destination, transform.position,
            Mathf.Pow(0.9f, Time.deltaTime * moveSpeed));
    }

    /*
     * Smoothly rotates a Gameobject from the start rotation to the new rotation at a constant speed
     * 
     * @param transform The start rotation
     * @param newRotation The end rotation
     * @param turnRate The speed to rotate the object
     */
    public static void SmoothRotateTowards(this Transform transform, Vector3 newRotation, float turnRate)
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(newRotation),
            turnRate * Time.deltaTime);
    }

    /*
     * Compares two positions to see if they are equal within a certain tolerance value
     * 
     * @param transform The first position
     * @param target The second position
     * @param tolerance How close the two values need to be to be considered equal
     * @return bool True if the distance is within the tolerance
     */
    public static bool ComparePosition(this Transform transform, Vector3 target, float tolerance)
    {
        return Vector3.Distance(transform.position, target) < tolerance;
    }
    
    /*
     * Rotates a GameObject in the direction of another object at a certain speed
     * 
     * @param transform The start transform
     * @param target The end transform
     * @param rotateSpeed The speed at which the rotation occurs
     */
    public static void RotateTowards(this Transform transform, Transform target, float rotateSpeed)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed );
    }
    
    /*
     * Changes the layer a GameObject is currently on as well as all its children
     * 
     * @param transform The GameObject having its layer changed
     * @param layerMask The layer to change the GameObjects to 
     */
    public static void ChangeLayersRecursively(this Transform transform, LayerMask layerMask)
    {
        int layer = (int) Mathf.Log(layerMask, 2);
        transform.gameObject.layer = layer;
        foreach(Transform child in transform)
        {            
            child.ChangeLayersRecursively(layerMask);
        }
    }

    // TODO: Is not very precise
    /*
     * Centers a GameObject on a particular transform
     * 
     * @param transform The GameObject transform to center the GameObject on
     * @return Vector3 The location to place the GameObject on
     */
    public static Vector3 Center(this Transform transform)
    {
        Vector3 sum = transform.position;
        
        foreach (Transform child in transform)
        {
            //Debug.Log(child.Center().ToString("F4"));
            sum += (transform.position - child.Center()) * 0.5f;
        }

        Debug.Log(sum.ToString("F4"));

        return sum;
    }
}
