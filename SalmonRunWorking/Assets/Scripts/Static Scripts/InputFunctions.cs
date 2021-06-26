using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Functions that are involved with user input such as mouse / cursor movement
 * 
 * Authors: Benjamin Person (Editor 2020)
 */
public static class InputFunctions
{
    /*
     * Checks for whether the mouse is moving
     * 
     * @return bool True if the mouse is currently moving
     */
    public static bool CheckForMouseMvt()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        return false;
    }

    /*
     * Gets the mouses position relative to the origin, as a point in Unity space
     * 
     * @param origin The location of the mouse
     * @return Vector3 The location of the mouse in Unity space
     */
    public static Vector3 GetMouseVector(GameObject origin)
    {
        Vector3 target = Input.mousePosition;

        Vector3 object_pos = Camera.main.WorldToScreenPoint(origin.transform.position);

        target.x = target.x - object_pos.x;
        target.y = target.y - object_pos.y;
        target.z = target.z - object_pos.z;

        return target;
    }
    
    /*
     * Checks if an object in the scene is moving
     * 
     * @return bool True if the object is currently moving
     */
    public static bool IsMoving()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            return true;
        }
        return false;
    }

    /*
     * Turns the cursor on or off depending on the bool input
     * 
     * @param cursorOn Whether or not the cursor should be on (True) or off (False)
     */
    public static void ToggleCursor(bool cursorOn)
    {
        Cursor.visible = cursorOn;
        if (cursorOn)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
